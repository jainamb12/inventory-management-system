using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ContactEmail { get; set; }

        public string Phone { get; set; }

        // One Supplier provides Many Products
        public List<Product>? Products { get; set; }
    }
}