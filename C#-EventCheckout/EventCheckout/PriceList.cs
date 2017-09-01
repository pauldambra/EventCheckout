using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventCheckout
{
    internal class PriceList
    {
        private readonly Dictionary<string, int> _prices = new Dictionary<string, int>();

        public void Apply(IEnumerable<PriceAdded> events)
        {
            events.ToList()
                .ForEach(priceAdded => _prices[priceAdded.SKU] = priceAdded.Price);
        }

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

        public static async Task<PriceList> From(IEventStoreConnection eventStoreConnection)
        {
            var streamEvents = await ReadWholePricesStream(eventStoreConnection);

            var pricesAdded = streamEvents
                .Select(resEv => resEv.Event)
                .Where(e => e.EventType == nameof(PriceAdded))
                .Select(ev => ev.Data)
                .Select(d => Encoding.UTF8.GetString(d))
                .Select(JsonConvert.DeserializeObject<PriceAdded>);
            
            var priceList = new PriceList();
            priceList.Apply(pricesAdded);
            
            return priceList;
        }

        private static async Task<List<ResolvedEvent>> ReadWholePricesStream(IEventStoreConnection eventStoreConnection)
        {
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice =
                    await eventStoreConnection.ReadStreamEventsForwardAsync("prices", nextSliceStart,
                        5, false);

                nextSliceStart = (int) currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);
            return streamEvents;
        }
        
        public class CannotProvidePriceForUnknownSKU : Exception
        {
            // ReSharper disable once SuggestBaseTypeForParameter
            public CannotProvidePriceForUnknownSKU(string sku, KeyNotFoundException knfe)
                :base($"Cannot provide a price for {sku}. It has never had a price added", knfe)
            {}
        }
    }
}