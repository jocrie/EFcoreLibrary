using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Entities;

public class Book
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
    [Required]
    public string ISBN { get; set; }
    
    // Seen as additional info
    public int? ReleaseYear { get; set; } = null;

    // Navigation properties
    // One book can only be rented by one person at a time
    // It does not seem important to retrieve info who has borrowed a book at the moment so no navigation property for borrower 
    public Loan? Loan { get; set; }
    
}
