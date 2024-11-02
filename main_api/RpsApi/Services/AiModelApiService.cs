using System.Net;
using System.Net.Http.Headers;
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
    public async Task<GestureType> AnalyzeGesture(string filePath)
    {
        using var content = new MultipartFormDataContent();
        var fileStream = new FileStream(filePath, FileMode.Open);
        var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType = MimeTypeHelper.GetImageMimeType(filePath);
        content.Add(streamContent, "file", Path.GetFileName(filePath));
        var predictionResponseDto = await PostAsync<PredictionResponseDto>(_settings.Endpoint, content);
        if (predictionResponseDto is null)
        {
            throw new AiModelApiException("Failed to analyze gesture");
        }
        return predictionResponseDto.Prediction;
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
            throw new AiModelApiException("Failed to analyze gesture", e);
        }
        return response;
    }
}