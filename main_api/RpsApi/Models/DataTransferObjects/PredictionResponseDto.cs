using RpsApi.Models.Enums;

namespace RpsApi.Models.DataTransferObjects;

public class PredictionResponseDto
{
    public GestureType Prediction { get; set; }
}