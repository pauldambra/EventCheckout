﻿using Xunit;

namespace EventCheckout.Tests
{
    public class PriceListTests
    {
        [Fact]
        public void CanAddAPrice()
        {
            PriceAdded[] events =
            {
                new PriceAdded("A", 50)
            };

            var priceList = new PriceList();
            priceList.Apply(events);

            var price = priceList.PriceFor("A");
            Assert.Equal(50, price);
        }

        [Fact]
        public void CanGetOnlyTheLatestPrice()
        {
            PriceAdded[] events =
            {
                new PriceAdded("A", 50),
                new PriceAdded("A", 20)
            };

            var priceList = new PriceList();
            priceList.Apply(events);

            var price = priceList.PriceFor("A");
            Assert.Equal(20, price);
        }

        [Fact]
        public void CanGetPriceForMultipleSKU()
        {
            PriceAdded[] events =
            {
                new PriceAdded("A", 50),
                new PriceAdded("B", 30)
            };

            var priceList = new PriceList();
            priceList.Apply(events);

            Assert.Equal(50, priceList.PriceFor("A"));
            Assert.Equal(30, priceList.PriceFor("B"));
        }

        [Fact]
        public void ThrowsANiceExceptionWhenSKUIsUnknown()
        {
            PriceAdded[] events =
            {
                new PriceAdded("A", 50),
                new PriceAdded("B", 30)
            };

            var priceList = new PriceList();
            priceList.Apply(events);

            var ex = Assert.Throws<PriceList.CannotProvidePriceForUnknownSKU>(
                () => priceList.PriceFor("C"));
            
            Assert.Contains("price for C", ex.Message);
        }
    }
}