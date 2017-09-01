namespace EventCheckout
{
    internal class ItemScanned : Event
    {
        public string SKU { get; }
        public int Price { get; }

        public ItemScanned(string sku, int price)
        {
            SKU = sku;
            Price = price;
        }
    }
}