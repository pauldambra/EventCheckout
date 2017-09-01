using System;
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
            await checkout.Scan("A", correlationId);
            
            var slice = await 
                _eventStoreConnection.ReadStreamEventsBackwardAsync($"basket-{correlationId}", StreamPosition.End, 1, false);
            Assert.Equal(SliceReadStatus.Success, slice.Status);
            
            var returnedEvent = slice.Events[0].Event;
            var returnedData = JsonConvert.DeserializeObject<ItemScanned>(Encoding.UTF8.GetString(returnedEvent.Data));
            Assert.Equal("A", returnedData.SKU);
        }
    }
}