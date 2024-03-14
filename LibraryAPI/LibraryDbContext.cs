using LibraryAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) :base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // To remove the automatic creation of the unique index
        modelBuilder.Entity<Loan>().HasIndex(l => l.BookId).IsUnique(false);
        // Define that ISBNs must be unique
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();
        // Define that SSN is unique
        modelBuilder.Entity<Borrower>()
            .HasIndex(b => b.SocialSecurityNumber)
            .IsUnique();
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Borrower> Borrowers { get; set; }
    public DbSet<Loan> Loans { get; set; }

}
