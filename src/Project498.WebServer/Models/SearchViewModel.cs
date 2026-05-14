using Project498.WebServer.Services;

namespace Project498.WebServer.Models;

public class SearchViewModel
{
    public string Query { get; set; } = "";
    public List<Comic> Comics { get; set; } = new();
    public List<DcCharacterDto> DcCharacters { get; set; } = new();
    public List<MarvelCharacterDto> MarvelCharacters { get; set; } = new();
}
