using Microsoft.Build.Framework;

namespace Mairala.Areas.Admin.ViewModels
{
    public class CreatePositionVM
    {
        [Required]
        public string Name { get; set; }
    }
}
