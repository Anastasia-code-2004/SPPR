using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Web253502Nikolaychik.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        // Навигационное свойство для Commodities
        [JsonIgnore]
        public ICollection<Commodity> Commodities { get; set; }
    }
}
