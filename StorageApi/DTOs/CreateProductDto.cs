using System.ComponentModel.DataAnnotations;

namespace StorageApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public int Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Shelf { get; set; } = string.Empty;

        [Range(0, 100000)]
        public int Count { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}