using System.ComponentModel.DataAnnotations;

namespace MovieTicket.Models
{
    public class AddMovieRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Genre { get; set; }

        [Required]
        [StringLength(100)]
        public string Cast { get; set; }
    }
}
