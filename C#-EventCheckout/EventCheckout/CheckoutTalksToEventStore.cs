
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Xunit;

namespace EventCheckout
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

    public class ItemScanned
    {
        public ItemScanned(string sku)
        {
            SKU = sku;
        }

        public string SKU { get; }
    }

    public class Checkout
    {
        private readonly IEventStoreConnection _eventStoreConnection;

        private Checkout(IEventStoreConnection eventStoreConnection)
        {
            _eventStoreConnection = eventStoreConnection;
        }

        public static async Task<Checkout> With(IEventStoreConnection eventStoreConnection)
        {
            await eventStoreConnection.ConnectAsync();
            return new Checkout(eventStoreConnection);
        }

        public async Task Scan(string sku, Guid correlationId)
        {
            var @event = new ItemScanned(sku); 
            var itemScannedEventData = new EventData(
                Guid.NewGuid(), 
                nameof(ItemScanned), 
                true, 
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                Encoding.UTF8.GetBytes(correlationId.ToString()));
            
            await _eventStoreConnection.AppendToStreamAsync($"basket-{correlationId}", ExpectedVersion.Any,
                itemScannedEventData);
        }
    }
}