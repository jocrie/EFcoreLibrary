using LibraryAPI.Entities;
using LibraryAPI.Models;

namespace LibraryAPI.Mappers;

public interface ILibraryMapper
{
    Book MapBook(CreateBookRequest book);
    BookDto MapBookDto(Book book, bool available = true);
    Borrower MapBorrower(CreateBorrowerRequest borrower);
    BorrowerDto MapBorrowerDto(Borrower borrower);
    public Loan MapLoan(int borrowerId, int bookId);
    public LoanDto MapLoanDto(Loan loan);
}

public class LibraryMapper : ILibraryMapper
{
    public Book MapBook(CreateBookRequest book)
    {
        return new Book()
        {
            Title = book.Title,
            ISBN = book.ISBN,
            ReleaseYear = book.ReleaseYear
        };
    }

    public BookDto MapBookDto(Book book, bool available = true) 
    {
        return new BookDto()
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            ReleaseYear = book.ReleaseYear,
            Available = available, 
        };
    }

    public Borrower MapBorrower(CreateBorrowerRequest borrower)
    {
        return new Borrower()
        {
            FirstName = borrower.FirstName,
            LastName = borrower.LastName,
            SocialSecurityNumber = borrower.SocialSecurityNumber,
        };
    }

    public BorrowerDto MapBorrowerDto(Borrower borrower)
    {
        return new BorrowerDto()
        {
            Id = borrower.Id,
            FirstName = borrower.FirstName,
            LastName = borrower.LastName,
            SocialSecurityNumber= borrower.SocialSecurityNumber,
        };
    }

    public Loan MapLoan(int borrowerId, int bookId)
    {
        return new Loan()
        {
            BookId = bookId,
            BorrowerId = borrowerId,
            LoanDate = DateTime.Now,
        };
    }

    public LoanDto MapLoanDto(Loan loan)
    {
        return new LoanDto()
        {
            LoanDate = loan.LoanDate,
            ReturnDate = loan.ReturnDate,
            BookId = loan.BookId,
            BorrowerId = loan.BorrowerId,
        };
    }
}
