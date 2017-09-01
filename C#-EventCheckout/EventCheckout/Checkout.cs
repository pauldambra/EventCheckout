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
        private readonly PriceList _priceList;

        private Checkout(IEventStoreConnection eventStoreConnection, PriceList priceList)
        {
            _eventStoreConnection = eventStoreConnection;
            _priceList = priceList;
        }

        public static async Task<Checkout> With(IEventStoreConnection eventStoreConnection)
        {
            await eventStoreConnection.ConnectAsync();
            var priceList = await PriceList.From(eventStoreConnection);
            return new Checkout(eventStoreConnection, priceList);
        }

        public async Task Scan(string sku, Guid correlationId)
        {
            var @event = new ItemScanned(sku, _priceList.PriceFor(sku)); 
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