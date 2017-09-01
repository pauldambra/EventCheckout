using System.Linq;

namespace EventCheckout
{
    internal class Basket
    {
        public void Apply(Event[] events) =>
            events.Select(e => (dynamic) e)
                .ToList()
                .ForEach(d => Apply(d));

        private void Apply(ItemScanned itemScanned) => Total += itemScanned.Price;
        private void Apply(DiscountEarned discountEarned) => Total -= discountEarned.Amount;

        public int Total { get; private set; }
    }
}