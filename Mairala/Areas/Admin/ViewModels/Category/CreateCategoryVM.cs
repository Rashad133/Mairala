using System.ComponentModel.DataAnnotations;

namespace Mairala.Areas.Admin.ViewModels
{
    public class CreateCategoryVM
    {
        [Required]
        public string Name { get; set; }
    }
}
