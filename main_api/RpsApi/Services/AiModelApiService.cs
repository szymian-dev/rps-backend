using RpsApi.Models.Enums;
using RpsApi.Models.Interfaces.IServices;

namespace RpsApi.Services;

public class AiModelApiService(HttpClient httpClient) : IAiModelApiService
{
    public async Task<GestureType> AnalyzeGesture(string filePath)
    {
         throw new NotImplementedException();
    }
}