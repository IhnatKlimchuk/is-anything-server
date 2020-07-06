using System.ComponentModel.DataAnnotations;

namespace IsAnythingServer.Controllers.Api
{
    public class PutRecordRequestDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string Subject { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string Predicate { get; set; }
        [Required]
        public bool Value { get; set; }
    }
}
