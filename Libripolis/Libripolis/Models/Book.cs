using Microsoft.AspNetCore.Identity;

namespace Libripolis.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Pages { get; set; }

        public int BookTypeId { get; set; }
        public virtual BookType? BookType { get; set; }

        public string? userId { get; set; }
        public virtual IdentityUser? user { get; set; }

    }
}
