using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Libripolis.Models;

namespace Libripolis.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Libripolis.Models.Book>? Book { get; set; }
        public DbSet<Libripolis.Models.BookType>? BookType { get; set; }
        public DbSet<Libripolis.Models.Borrow>? Borrow { get; set; }
        public DbSet<Libripolis.Models.BorrowReport>? BorrowReport { get; set; }
    }
}