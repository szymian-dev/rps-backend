using RpsApi.Models.Enums;

namespace RpsApi.Models.Interfaces.IServices;

public interface IAiModelApiService
{
    Task<GestureType> AnalyzeGesture(string filePath);   
}