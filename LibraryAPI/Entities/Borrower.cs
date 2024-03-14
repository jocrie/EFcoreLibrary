using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Entities;

public class Borrower
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string SocialSecurityNumber { get; set; }

    // Navigation properties
    // One Borrower can have several loans at the same time
    public ICollection<Loan>? Loans { get; set; }
}
