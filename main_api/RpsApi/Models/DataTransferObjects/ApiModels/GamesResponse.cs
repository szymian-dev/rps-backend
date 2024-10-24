namespace RpsApi.Models.DataTransferObjects;

public class GamesResponse
{
    public List<GameInfoDto> Games { get; set; }
    public int TotalGames { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}