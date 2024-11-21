using RpsApi.Models.DataTransferObjects;
using RpsApi.Models.DataTransferObjects.ApiModels;
using RpsApi.Models.Enums;

namespace RpsApi.Models.Interfaces.IServices;

public interface IAiModelApiService
{
    Task<GestureType?> AnalyzeGesture(string filePath, int modelId);   
    Task<List<AiModelDto>> GetAiModels();
    Task<bool> GiveFeedback(int modelId, bool wrongPrediction);
}