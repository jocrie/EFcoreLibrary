using LibraryAPI.Entities;
using LibraryAPI.Mappers;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers;

[Route("api")]
[ApiController]
public class LibraryController : ControllerBase
{
    private readonly ILibraryMapper _libraryMapper;
    private readonly LibraryDbContext _dbContext;

    public LibraryController(LibraryDbContext dbContext, ILibraryMapper libraryMapper)
    {
        _dbContext = dbContext;
        _libraryMapper = libraryMapper;
    }

    [HttpPost("borrower")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDto>> AddBorrower(CreateBorrowerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var borrower = _libraryMapper.MapBorrower(request);
        _dbContext.Borrowers.Add(borrower);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Could not save borrower to database: " + ex.InnerException?.Message);
        }

        var borrowerDto = _libraryMapper.MapBorrowerDto(borrower);
        //For simplicity the url is not included here
        return new ObjectResult(borrowerDto) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpDelete("borrower/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> DeleteBorrower(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid id");

        var borrower = await _dbContext.FindAsync<Borrower>(id);

        if (borrower is null)
            return NotFound();

        try
        {
            _dbContext.Borrowers.Remove(borrower);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Could not remove borrower: " + ex.InnerException?.Message);
        }

        return NoContent();
    }

    [HttpDelete("book/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookDto>> DeleteBook(int id)
    {
        if (id <= 0)
            return BadRequest("Invalid id");

        var book = await _dbContext.FindAsync<Book>(id);

        if (book is null)
            return NotFound();

        // Check if available to make sure that only returned books can be deleted
        var bookAvailable = !_dbContext.Loans.Any(l => l.BookId == id && l.ReturnDate == null);

        if (!bookAvailable)
            return Conflict("Return book before deleting it");

        try
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Could not remove book: " + ex.InnerException?.Message);
        }

        return NoContent();
    }

    [HttpGet("book")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
    {
        try
        {
            var books = await _dbContext.Books.AsNoTracking()
            .Select(b => _libraryMapper.MapBookDto(b, !_dbContext.Loans.Any(l => l.BookId == b.Id && l.ReturnDate == null)))
            .ToListAsync();

            // No null check needed here as books will return
            // in an empty list incase the db is empty

            return Ok(books);
        }
        catch (Exception ex)
        {
            // Log the exception for further analysis.
            Console.Error.WriteLine($"An unexpected error occurred: {ex}");

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [HttpGet("book/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        try
        {
            var book = await _dbContext.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null)
                return NotFound();

            // Check if available
            var bookAvailable = !_dbContext.Loans.Any(l => l.BookId == id && l.ReturnDate == null);

            return _libraryMapper.MapBookDto(book, bookAvailable);
        }
        catch (Exception ex)
        {
            // Log the exception for further analysis.
            Console.Error.WriteLine($"An unexpected error occurred: {ex}");

            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [HttpPost("book")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookDto>> AddBook(CreateBookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var book = _libraryMapper.MapBook(request);
        _dbContext.Books.Add(book);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Could not save book to database: " + ex.InnerException?.Message);
        }

        var bookDto = _libraryMapper.MapBookDto(book);

        return CreatedAtAction(nameof(GetById), new { id = bookDto.Id}, bookDto);
    }

    [HttpPost("rent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Loan>> RentBook(int borrowerId, int bookId)
    {
        if (borrowerId <= 0 || bookId <= 0)
            return BadRequest("Invalid borrowerId and/ or bookId");

        var borrower = await _dbContext.FindAsync<Borrower>(borrowerId);
        var book = await _dbContext.FindAsync<Book>(bookId);

        if (borrower is null)
            return NotFound("borrowerId not found");

        if (book is null)
            return NotFound("bookId not found");
        using (var transaction = _dbContext.Database.BeginTransaction())
        { 
            try
            {
                // Check if book available for rent
                var bookAvailable = !_dbContext.Loans.Any(l => l.BookId == bookId && l.ReturnDate == null);

                if (!bookAvailable)
                    return Conflict("Book already on loan, therefore not available");

                var loan = _libraryMapper.MapLoan(borrowerId, bookId);
                _dbContext.Loans.Add(loan);

                await _dbContext.SaveChangesAsync();

                transaction.Commit();

                var loanDto = _libraryMapper.MapLoanDto(loan);

                //For simplicity the url is not included here
                return new ObjectResult(loanDto) { StatusCode = StatusCodes.Status201Created };
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Could not add loan: " + ex.InnerException?.Message);
            }
        }
    }

    [HttpPut("return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoanDto>> ReturnBook(int bookId)
    {
        var book = await _dbContext.FindAsync<Book>(bookId);

        if (book is null)
            return NotFound("bookId not found");

        // Check if book on loan and can therefore be returned
        var bookReturn = _dbContext.Loans.SingleOrDefault(l => l.BookId == bookId && l.ReturnDate == null);

        if (bookReturn is null)
            return Conflict("Book not currently on loan, therefore no return possible");

        bookReturn.ReturnDate = DateTime.Now;
        
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("Could not return book: " + ex.InnerException?.Message);
        }

        var returnDto = _libraryMapper.MapLoanDto(bookReturn);
        //For simplicity the url is not included here
        return returnDto;
    }
}
