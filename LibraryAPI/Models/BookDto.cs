namespace LibraryAPI.Models;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public int? ReleaseYear { get; set; }
    public bool Available { get; set; }
}
