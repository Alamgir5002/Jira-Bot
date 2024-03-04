using System.ComponentModel.DataAnnotations;

namespace Jira_bot.Models
{
    /// <summary>
    /// This class respresents Source Details entity
    /// </summary>
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
