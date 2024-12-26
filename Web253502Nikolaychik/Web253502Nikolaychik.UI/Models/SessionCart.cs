using System.Text.Json.Serialization;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.UI.Controllers;
using Web253502Nikolaychik.UI.Extensions;

namespace Web253502Nikolaychik.UI.Models
{
    public class SessionCart : Domain.Entities.Cart
    {
        public static Domain.Entities.Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
            SessionCart cart = session?.Get<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        [JsonIgnore]
        public ISession? Session { get; set; }
        public override void AddToCart(Commodity commodity)
        {
            base.AddToCart(commodity);
            Session?.Set("Cart", this);
        }
        public override void RemoveItem(int id)
        {
            base.RemoveItem(id);
            Session?.Set("Cart", this);
        }
        public override void ClearAll()
        {
            base.ClearAll();
            Session?.Remove("Cart");
        }
    }
}
