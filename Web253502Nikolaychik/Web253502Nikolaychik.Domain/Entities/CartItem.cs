using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web253502Nikolaychik.Domain.Entities
{
    public class CartItem
    {
        public Commodity Commodity { get; set; }
        public int Amount { get; set; } = 1;
    }
}
