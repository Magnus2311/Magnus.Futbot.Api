using System.ComponentModel.DataAnnotations;

namespace Magnus.Futbot.Api.Models.Requests
{
    public class AddPlayerToListRequest
    {
        [Required]
        public string PlayerId { get; set; } = string.Empty;
    }
}
