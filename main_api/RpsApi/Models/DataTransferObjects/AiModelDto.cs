namespace RpsApi.Models.DataTransferObjects;

public class AiModelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}