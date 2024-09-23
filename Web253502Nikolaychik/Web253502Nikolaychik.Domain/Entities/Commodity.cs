using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web253502Nikolaychik.Domain.Entities
{
    public class Commodity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? ImageMimeType { get; set; }

        // Навигационное свойство на Category
        public Category? Category { get; set; }
    }
}
