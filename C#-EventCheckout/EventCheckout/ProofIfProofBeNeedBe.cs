using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Xunit;

namespace EventCheckout
{
    public class ProofIfProofBeNeedBe
    {
        [Fact]
        public async Task TheExampleCanRun()
        {
            var connection =
                EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();

            var myEvent = new EventData(Guid.NewGuid(), "testEvent", false,
                Encoding.UTF8.GetBytes("some data"),
                Encoding.UTF8.GetBytes("some metadata"));

            await connection.AppendToStreamAsync("test-stream",
                ExpectedVersion.Any, myEvent);

            var streamEvents = await 
                connection.ReadStreamEventsForwardAsync("test-stream", 0, 1, false);

            var returnedEvent = streamEvents.Events[0].Event;

            var returnedData = Encoding.UTF8.GetString(returnedEvent.Data);
            var returnedMetaData = Encoding.UTF8.GetString(returnedEvent.Metadata);
            
            Assert.Equal("some data", returnedData);
            Assert.Equal("some metadata", returnedMetaData);
        }
    }
}