using System.ComponentModel.DataAnnotations.Schema;

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
    
    public string SeriesName { get; set; } = "";
    
    public int VolumeNumber { get; set; }
    
    public int IssueNumber { get; set; }
    
    // temporary while setting up database to link with frontend
    [NotMapped]
    public int ProgressPercent { get; set; }

    [NotMapped]
    public int CurrentPage { get; set; } = 1;
    
    // temporary while setting up database to link with frontend
    [NotMapped]
    public string Shelf { get; set; } = "";

    public bool IsIReadPick { get; set; }
    
    
}
