using RpsApi.Models.Enums;

namespace RpsApi.Models.DataTransferObjects;

public class GestureInfoDto
{
    public int FileId { get; set; }
    public GestureType? Type { get; set; }
}