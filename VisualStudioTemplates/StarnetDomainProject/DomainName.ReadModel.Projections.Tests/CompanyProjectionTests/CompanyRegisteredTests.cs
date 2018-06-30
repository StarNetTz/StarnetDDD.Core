using $ext_projectname$.PL.Events;
using NUnit.Framework;
using Starnet.Projections.Testing;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    [TestFixture]
    public class CompanyRegisteredTests : ProjectionSpecification<CompanyProjection, Company>
    {
        string Id;

        [SetUp]
        public void SetUp()
        {
            Id = $"Company-{Guid.NewGuid()}";
        }

        [Test]
        public async Task can_project_event()
        {
            await Given(new CompanyRegistered() { Id = Id, Name = "Betting Shop Royal", Address = new PL.Address { Street = "My street", City = "My City", Country = "My Country", State = "TK", ZipCode = "75000" }, VATId = "789" });
            await Expect(new Company() { Id = Id, Name = "Betting Shop Royal", Address = new PL.Address { Street = "My street", City = "My City", Country = "My Country", State = "TK", ZipCode = "75000" }, VATId = "789" });
        }
    }
}
