using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace PulseHub.Application.DTOs
{
    /// <summary>
    /// Dados necessários para criar ou atualizar um produto.
    /// </summary>
    public class ProductRequestDto
    {
        /// <summary>
        /// Nome do produto.
        /// </summary>
        /// <example>iPhone 11</example>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Descrição detalhada do produto.
        /// </summary>
        /// <example>Smartphone Apple iPhone 11 128GB Preto</example>
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? Description { get; set; }

        /// <summary>
        /// Preço do produto.
        /// </summary>
        /// <example>1999.99</example>
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Quantidade disponível em estoque.
        /// </summary>
        /// <example>10</example>
        [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
        public int Stock { get; set; }

        /// <summary>
        /// Define se o produto está ativo.
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;
    }
}
