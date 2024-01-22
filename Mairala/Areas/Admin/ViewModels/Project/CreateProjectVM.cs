using Mairala.Models;
using Microsoft.Build.Framework;

namespace Mairala.Areas.Admin.ViewModels
{
    public class CreateProjectVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        
        public IFormFile? Photo { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
