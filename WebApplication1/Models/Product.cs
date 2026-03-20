using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Foreign Key for Category
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // NEW: Foreign Key for Supplier
        [Required]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

    }
}
