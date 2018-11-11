using $ext_projectname$.PL.Events;
using NUnit.Framework;
using Starnet.Projections.Testing;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    [TestFixture]
    public class OrganizationRegisteredTests : ProjectionSpecification<OrganizationProjection, Organization>
    {
        string Id;

        [SetUp]
        public void SetUp()
        {
            Id = $"Organization-{Guid.NewGuid()}";
        }

        [Test]
        public async Task can_project_event()
        {
            await Given(new OrganizationRegistered() { Id = Id, Name = "Betting Shop Royal", Address = new PL.Address { Street = "My street", City = "My City", Country = "My Country", State = "TK", ZipCode = "75000" }});
            await Expect(new Organization() { Id = Id, Name = "Betting Shop Royal", Address = new PL.Address { Street = "My street", City = "My City", Country = "My Country", State = "TK", ZipCode = "75000" }});
        }
    }
}
