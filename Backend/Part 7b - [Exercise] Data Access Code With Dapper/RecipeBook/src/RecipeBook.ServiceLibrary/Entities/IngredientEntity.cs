using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeBook.ServiceLibrary.Entities
{
    public class IngredientEntity
    {
        public Guid RecipeId { get; set; }
        public int OrdinalPosition { get; set; }
        public string Unit { get; set; }
        public float Quantity { get; set; }
        public string Ingredient { get; set; }
    }
}
