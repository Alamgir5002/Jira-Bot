using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class SourceDetails
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string SourceURL { get; set; }
        [Required]
        public string SourceToken { get; set; }

    }
}
