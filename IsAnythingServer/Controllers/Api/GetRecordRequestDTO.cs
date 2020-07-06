using System.ComponentModel.DataAnnotations;

namespace IsAnythingServer.Controllers.Api
{
    public class GetRecordRequestDTO
    {
        [Required]
        [Display(Name = "subject")]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "predicate")]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string Predicate { get; set; }
    }
}
