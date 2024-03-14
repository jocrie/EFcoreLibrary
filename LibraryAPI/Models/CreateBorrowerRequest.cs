using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models;

public class CreateBorrowerRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SocialSecurityNumber { get; set; }
}
