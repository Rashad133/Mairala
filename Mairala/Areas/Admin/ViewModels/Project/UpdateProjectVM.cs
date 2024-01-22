using Mairala.Models;
using System.ComponentModel.DataAnnotations;

namespace Mairala.Areas.Admin.ViewModels
{
    public class UpdateProjectVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string? Image { get; set; }
        public IFormFile? Photo { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }

    }
}
