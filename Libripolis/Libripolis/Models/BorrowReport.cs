using System;
using System.ComponentModel.DataAnnotations;

namespace Libripolis.Models
{
    public class BorrowReport
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public Book Book { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
