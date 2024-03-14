using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities;

public class Loan
{
    public int Id { get; set; }
    [Required]
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; } = null;

    // Foreign keys
    public int BookId { get; set; }
    public int BorrowerId { get; set; }

    // Navigation properties
    [JsonIgnore]
    public Book Book { get; set; }
    [JsonIgnore]
    public Borrower Borrower { get; set;}
}
