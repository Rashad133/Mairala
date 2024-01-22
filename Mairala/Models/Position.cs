namespace Mairala.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Member>? Members { get; set; }
    }
}
