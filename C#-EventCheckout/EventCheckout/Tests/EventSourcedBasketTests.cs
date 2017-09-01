using Xunit;

namespace EventCheckout.Tests
{
    public class EventSourcedBasketTests
    {
        [Fact]
        public void AnAIsFifty()
        {
            Event[] events = {
                new ItemScanned("A", 50)
            };
            
            var basket = new Basket();
            basket.Apply(events);
            
            Assert.Equal(50, basket.Total);
        }
        
        [Fact]
        public void TwoAsAreOneHundred()
        {
            Event[] events = {
                new ItemScanned("A", 50),
                new ItemScanned("A", 50)
            };
            
            var basket = new Basket();
            basket.Apply(events);
            
            Assert.Equal(100, basket.Total);
        }
        
        [Fact]
        public void ThreeAsAreOneTwenty()
        {
            Event[] events = {
                new ItemScanned("A", 50),
                new ItemScanned("A", 50),
                new ItemScanned("A", 50),
                new DiscountEarned(30)
            };
            
            var basket = new Basket();
            basket.Apply(events);
            
            Assert.Equal(120, basket.Total);
        }
    }
}