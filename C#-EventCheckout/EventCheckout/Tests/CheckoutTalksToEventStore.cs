using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Xunit;

namespace EventCheckout.Tests
{
    public class CheckoutTalksToEventStore
    {
        private readonly IEventStoreConnection _eventStoreConnection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
        
        [Fact]
        public async Task ScanningAnItemWritesToABasketStream()
        {
            var correlationId = Guid.NewGuid();
            var checkout = await Checkout.With(_eventStoreConnection);
            await EnsurePricesAvailable(_eventStoreConnection);
            
            await checkout.Scan("A", correlationId);
            
            var itemScanned = await MostRecentItemScannedIntoBasket(correlationId);
            Assert.Equal("A", itemScanned.SKU);
        }

        [Fact]
        public async Task CheckoutCanScanAtDifferentPrices()
        {
            var basketOne = Guid.NewGuid();
            var basketTwo = Guid.NewGuid();
            
            var checkout = await Checkout.With(_eventStoreConnection);

            await EnsurePricesAvailable(_eventStoreConnection);
            
            await checkout.Scan("A", basketOne);
            await checkout.Scan("B", basketTwo);
            
            var basketOneA = await MostRecentItemScannedIntoBasket(basketOne);
            Assert.Equal(50, basketOneA.Price);
            
            var basketTwoB = await MostRecentItemScannedIntoBasket(basketTwo);
            Assert.Equal(30, basketTwoB.Price);
        }

        private async Task EnsurePricesAvailable(IEventStoreConnection eventStoreConnection)
        {
            var a = new PriceAdded("A", 50);
            var b = new PriceAdded("B", 30);
            
            var events = new[] { a, b }
                .Select(JsonConvert.SerializeObject)
                .Select(j => new EventData(Guid.NewGuid(), nameof(PriceAdded), true, Encoding.UTF8.GetBytes(j), null));

            await eventStoreConnection.AppendToStreamAsync("prices", ExpectedVersion.Any, events);
        }

        private async Task<ItemScanned> MostRecentItemScannedIntoBasket(Guid correlationId)
        {
            var slice = await
                _eventStoreConnection.ReadStreamEventsBackwardAsync($"basket-{correlationId}", StreamPosition.End, 1, false);
            Assert.Equal(SliceReadStatus.Success, slice.Status);

            var returnedEvent = slice.Events[0].Event;
            return JsonConvert.DeserializeObject<ItemScanned>(Encoding.UTF8.GetString(returnedEvent.Data));
        }
    }
}