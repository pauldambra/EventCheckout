using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventCheckout
{
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
            var @event = new ItemScanned(sku, 50); 
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