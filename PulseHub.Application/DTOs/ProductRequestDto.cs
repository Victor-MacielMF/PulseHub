using System.ComponentModel.DataAnnotations;

namespace PulseHub.Application.DTOs
{
    /// <summary>
    /// Data required to create or update a product.
    /// </summary>
    public class ProductRequestDto
    {
        /// <summary>
        /// Product name.
        /// </summary>
        /// <example>iPhone 11</example>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(150, ErrorMessage = "Name must be at most 150 characters.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Detailed product description.
        /// </summary>
        /// <example>Apple iPhone 11 128GB Black Smartphone</example>
        [MaxLength(500, ErrorMessage = "Description must be at most 500 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Product price.
        /// </summary>
        /// <example>1999.99</example>
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Available stock quantity.
        /// </summary>
        /// <example>10</example>
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        /// <summary>
        /// Defines whether the product is active.
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;
    }
}
