using Mairala.Models;
using System.ComponentModel.DataAnnotations;

namespace Mairala.Areas.Admin.ViewModels
{
    public class CreateMemberVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string? FaceLink { get; set; }
        public string? TwitLink { get; set; }
        public string? LinkedLink { get; set; }

        [Required]
        public IFormFile? Photo { get; set; }

        [Required]
        public int PositionId { get; set; }
        public List<Position> Positions { get; set; }

    }
}
