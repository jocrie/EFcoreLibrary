using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models;

public class CreateBookRequest
{
    public string Title { get; set; }
    public string ISBN { get; set; }
    public int? ReleaseYear { get; set; }
}
