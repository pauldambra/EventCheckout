namespace EventCheckout
{
    internal class PriceAdded : Event
    {
        public string SKU { get; }
        public int Price { get; }

        public PriceAdded(string sku, int price)
        {
            SKU = sku;
            Price = price;
        }
    }
}