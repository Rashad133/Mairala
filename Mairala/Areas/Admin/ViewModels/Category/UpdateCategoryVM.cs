using System.ComponentModel.DataAnnotations;

namespace Mairala.Areas.Admin.ViewModels
{
    public class UpdateCategoryVM
    {
        [Required]
        public string Name { get; set; }
    }
}
