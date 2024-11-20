using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.Enums;
using RpsApi.Models.Exceptions;
using RpsApi.Models.Interfaces.IServices;
using RpsApi.Models.Settings;
using RpsApi.Utils;

namespace RpsApi.Services;

public class AiModelApiService(HttpClient httpClient, IOptions<AiModelApiSettings> options, IJwtService jwtService) : IAiModelApiService
{
    private readonly AiModelApiSettings _settings = ValidateSettings(options.Value);
    public async Task<GestureType?> AnalyzeGesture(string filePath, int modelId)
    {
        using var content = new MultipartFormDataContent();
        var fileStream = new FileStream(filePath, FileMode.Open);
        var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType = MimeTypeHelper.GetImageMimeType(filePath);
        content.Add(streamContent, "file", Path.GetFileName(filePath));
        var url = QueryHelpers.AddQueryString(_settings.Endpoint, "model_id", modelId.ToString());
        var predictionResponseDto = await PostAsync<PredictionResponseDto>(url, content);
        if (predictionResponseDto is null)
        {
            throw new AiModelApiException("Failed to analyze gesture... No response");
        }
        return predictionResponseDto.Prediction;
    }

    public async Task<List<AiModelDto>> GetAiModels()
    {
        var url = _settings.ModelListEndpoint;
        var modelList = await GetAsync<List<AiModelDto>>(url);
        if (modelList is null)
        {
            throw new AiModelApiException("Failed to get models... No response");
        }
        return modelList;
    }

    public Task<bool> GiveFeedback(int modelId, bool wrongPrediction)
    {
        var url = QueryHelpers.AddQueryString(_settings.UpdateStatisticsEndpoint, "model_id", modelId.ToString());
        url = QueryHelpers.AddQueryString(url, "wrong_prediction", wrongPrediction.ToString());
        return PutAsync<bool>(url, null);
    }


    private static AiModelApiSettings ValidateSettings(AiModelApiSettings settings)
    {
        if (String.IsNullOrWhiteSpace(settings.Url))
        {
            throw new ArgumentException("Url is required");
        }
        if (String.IsNullOrWhiteSpace(settings.Endpoint))
        {
            throw new ArgumentException("Endpoint is required");
        }
        if (String.IsNullOrWhiteSpace(settings.ModelListEndpoint))
        {
            throw new ArgumentException("ModelListEndpoint is required");
        }
        if (String.IsNullOrWhiteSpace(settings.UpdateStatisticsEndpoint))
        {
            throw new ArgumentException("UpdateStatisticsEndpoint is required");
        }
        return settings;
    }

    private async Task<T?> PostAsync<T>(string url, HttpContent? content)
    {
        var jwtToken = jwtService.GetJwtForAiModelApi();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken.TokenType, jwtToken.Token);
        var response = await TryPostWithRetry(url, content);
        var responseObject = await response.Content.ReadFromJsonAsync<T>();
        return responseObject;
    }
    
    private async Task<T?> GetAsync<T>(string url)
    {
        var jwtToken = jwtService.GetJwtForAiModelApi();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken.TokenType, jwtToken.Token);
        var response = await TryGetWithRetry(url);
        var responseObject = await response.Content.ReadFromJsonAsync<T>();
        return responseObject;
    }
    
    private async Task<T?> PutAsync<T>(string url, HttpContent? content)
    {
        var jwtToken = jwtService.GetJwtForAiModelApi();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken.TokenType, jwtToken.Token);
        var response = await TryPutWithRetry(url, content);
        var responseObject = await response.Content.ReadFromJsonAsync<T>();
        return responseObject;
    }
    
    private async Task<HttpResponseMessage> TryPostWithRetry(string url, HttpContent? content)
    {
        HttpResponseMessage response = await httpClient.PostAsync(url, content);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var newToken = jwtService.CreateJwtForAiModelApi();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(newToken.TokenType, newToken.Token);
            response = await httpClient.PostAsync(url, content);
        }
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            HandleAiApiException(response);
        }
        return response;
    }
    
    private async Task<HttpResponseMessage> TryGetWithRetry(string url)
    {
        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var newToken = jwtService.CreateJwtForAiModelApi();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(newToken.TokenType, newToken.Token);
            response = await httpClient.GetAsync(url);
        }
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            HandleAiApiException(response);
        }
        return response;
    }
    
    private async Task<HttpResponseMessage> TryPutWithRetry(string url, HttpContent? content)
    {
        HttpResponseMessage response = await httpClient.PutAsync(url, content);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var newToken = jwtService.CreateJwtForAiModelApi();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(newToken.TokenType, newToken.Token);
            response = await httpClient.PutAsync(url, content);
        }
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            HandleAiApiException(response);
        }
        return response;
    }
    
    private static void HandleAiApiException(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException($"Unauthorized. {response.ReasonPhrase}");
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"Not found. {response.ReasonPhrase}");
        }
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new ForbiddenAccessException($"Forbidden. {response.ReasonPhrase}");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BadRequestException($"Bad request. {response.ReasonPhrase}");
        }
        throw new AiModelApiException($"AiModelApi: {response.StatusCode} {response.ReasonPhrase}");
    }
}