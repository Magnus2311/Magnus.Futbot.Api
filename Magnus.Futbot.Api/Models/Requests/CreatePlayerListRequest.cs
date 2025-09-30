using System.ComponentModel.DataAnnotations;

namespace Magnus.Futbot.Api.Models.Requests
{
    public class CreatePlayerListRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }
}
