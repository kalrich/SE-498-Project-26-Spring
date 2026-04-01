namespace Project498.WebServer.Models;

public class Comic
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Genre { get; set; } = "";
    public string SecondaryGenre { get; set; } = "";
    public string Description { get; set; } = "";

    public string CoverImagePath { get; set; } = "";
    public string PdfPath { get; set; } = "";

    public int ProgressPercent { get; set; }

    public string Shelf { get; set; } = "";

    public bool IsIReadPick { get; set; }
    
    
}