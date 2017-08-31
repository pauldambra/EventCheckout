﻿using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Xunit;

namespace EventCheckout
{
   
    public class ProofIfProofBeNeedBe
    {
        private readonly IEventStoreConnection _eventStoreConnection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));

        private async Task WriteBytesToStream(string streamName, string eventType, string eventData)
        {
            var myEvent = new EventData(Guid.NewGuid(), eventType, false,
                Encoding.UTF8.GetBytes(eventData),
                Encoding.UTF8.GetBytes("some metadata"));

            await _eventStoreConnection.AppendToStreamAsync(streamName,
                ExpectedVersion.Any, myEvent);
        }

        private async Task WriteJSONToStream(string streamName, string eventType, string eventData)
        {
            var myEvent = new EventData(Guid.NewGuid(), eventType, true,
                Encoding.UTF8.GetBytes(eventData),
                Encoding.UTF8.GetBytes("some metadata"));

            await _eventStoreConnection.AppendToStreamAsync(streamName,
                ExpectedVersion.Any, myEvent);
        }

        [Fact]
        public async Task TheExampleCanRun()
        {
            await _eventStoreConnection.ConnectAsync();

            await WriteBytesToStream("test-stream", "testEvent", "some data");

            var streamEvents = await 
                _eventStoreConnection.ReadStreamEventsBackwardAsync("test-stream", StreamPosition.End, 1, false);

            var returnedEvent = streamEvents.Events[0].Event;
            var returnedData = Encoding.UTF8.GetString(returnedEvent.Data);
            var returnedMetaData = Encoding.UTF8.GetString(returnedEvent.Metadata);
            
            Assert.Equal("some data", returnedData);
            Assert.Equal("some metadata", returnedMetaData);
        }

        [Fact]
        public async Task TheExampleCanRunWithJsonStored()
        {
            await _eventStoreConnection.ConnectAsync();

            await WriteJSONToStream("test-stream", "testEvent", "{\"some\": \"data\"}");
            
            var streamEvents = await 
                _eventStoreConnection.ReadStreamEventsBackwardAsync("test-stream", StreamPosition.End, 1, false);

            var returnedEvent = streamEvents.Events[0].Event;

            var returnedData = Encoding.UTF8.GetString(returnedEvent.Data);
            var returnedMetaData = Encoding.UTF8.GetString(returnedEvent.Metadata);
            
            Assert.Equal("{\"some\": \"data\"}", returnedData);
            Assert.Equal("some metadata", returnedMetaData);
        }
    }
}