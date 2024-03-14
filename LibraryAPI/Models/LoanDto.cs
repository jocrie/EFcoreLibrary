using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models
{
    public class LoanDto
    {
        public int BookId { get; set; }
        public int BorrowerId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; } = null;
        
    }
}
