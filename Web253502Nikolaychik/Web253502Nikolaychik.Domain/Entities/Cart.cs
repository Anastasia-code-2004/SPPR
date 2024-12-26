using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web253502Nikolaychik.Domain.Entities
{
    public class Cart
    {
        public Dictionary<int, CartItem> CartItems { get; set; } = [];
        public virtual void AddToCart(Commodity commodity)
        {
            if (CartItems.TryGetValue(commodity.Id, out CartItem? value))
            {
                value.Amount++;
            }
            else
            {
                CartItems[commodity.Id] = new CartItem { Commodity = commodity };
            }
        }
        public virtual void RemoveItem(int id)
        {
            if (CartItems.TryGetValue(id, out CartItem? value))
            {
                value.Amount--;
                if (value.Amount <= 0)
                {
                    CartItems.Remove(id);
                }
            }
        }
        public virtual void ClearAll() 
        { 
            CartItems.Clear();
        }
        public int Count { get => CartItems.Sum(item => item.Value.Amount); }
        public decimal TotalCost
        {
            get => CartItems.Sum(item => item.Value.Commodity.Price * item.Value.Amount);
        }
    }
}

