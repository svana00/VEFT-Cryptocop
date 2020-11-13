using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptocop.Software.API.Models.InputModels
{
    public class ShoppingCartItemInputModel
    {
        [Required]
        public string ProductIdentifier { get; set; }

        [Required]
        [Range(0.1, float.MaxValue)]
        public Nullable<float> Quantity { get; set; }
    }
}