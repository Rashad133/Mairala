namespace Mairala.Models
{
    public class Project
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
