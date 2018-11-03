using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Starnet.Projections.ES.IntegrationTests
{
    [TestFixture]
    class EventStoreProjectionBuilderTests
    {
        [Test]
        public void DevicesProjection_is_created()
        {
            var p = new EventStoreProjectionParameters {
                Name = "ProjectionDevices",
                SourceStreamNames = new List<string> { "$ce-Locations" },
                DestinationStreamName = "cp-Devices",
                EventsToInclude = new Type[] { typeof(LocationOpened) }
            };
            var proj = EventStoreProjectionBuilder.BuildProjectionDefinition(p);
           
            string expected = "fromStreams('$ce-Locations').when({LocationOpened: function(s,e){linkTo('cp-Devices', e);return s;}})";
            Assert.That(proj.Name, Is.EqualTo("ProjectionDevices"));
            Assert.That(proj.Source, Is.EqualTo(expected));
        }

        class LocationOpened
        {
        }
    }
}
