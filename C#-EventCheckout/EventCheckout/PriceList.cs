using System;
using System.Collections.Generic;
using System.Linq;

namespace EventCheckout
{
    internal class PriceList
    {
        private readonly Dictionary<string, int> _prices = new Dictionary<string, int>();

        public void Apply(Event[] events)
        {
            events.ToList().ForEach(e => Apply((dynamic) e));
        }

        private void Apply(PriceAdded priceAdded) => _prices[priceAdded.SKU] = priceAdded.Price;

        public int PriceFor(string sku)
        {
            try
            {
                return _prices[sku];
            }
            catch (KeyNotFoundException knfe)
            {
                throw new CannotProvidePriceForUnknownSKU(sku, knfe);
            }
            
        }
    }

    internal class CannotProvidePriceForUnknownSKU : Exception
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public CannotProvidePriceForUnknownSKU(string sku, KeyNotFoundException knfe)
            :base($"Cannot provide a price for {sku}. It has never had a price added", knfe)
        {}
    }
}