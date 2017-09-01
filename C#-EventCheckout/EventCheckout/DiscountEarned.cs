namespace EventCheckout
{
    internal class DiscountEarned : Event
    {
        public int Amount { get; }

        public DiscountEarned(int amount)
        {
            Amount = amount;
        }
    }
}