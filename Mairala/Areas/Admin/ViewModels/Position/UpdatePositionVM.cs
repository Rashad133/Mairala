using System.ComponentModel.DataAnnotations;

namespace Mairala.Areas.Admin.ViewModels
{
    public class UpdatePositionVM
    {
        [Required]
        public string Name { get; set; }
    }
}
