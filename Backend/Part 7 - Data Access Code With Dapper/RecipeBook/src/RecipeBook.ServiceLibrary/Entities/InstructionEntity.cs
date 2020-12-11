using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeBook.ServiceLibrary.Entities
{
    public class InstructionEntity
    {
        public Guid RecipeId { get; set; }
        public int OrdinalPosition { get; set; }
        public string Instruction { get; set; }
    }
}
