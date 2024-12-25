using Microsoft.AspNetCore.Identity;

namespace Libripolis.Models
{
    public class Borrow
    {
        public int Id { get; set; }
  
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }

        public string? userId { get; set; }
        public virtual IdentityUser? user { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}