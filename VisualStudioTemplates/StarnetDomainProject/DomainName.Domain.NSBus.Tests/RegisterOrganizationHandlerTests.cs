using $ext_projectname$.Domain.Organization;
using $ext_projectname$.PL.Commands;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class RegisterOrganizationHandlerTests
    {
        [Test]
        public async Task ShouldExecute()
        {
            var mock = new Mock<IOrganizationInteractor>();
            mock.Setup(svc => svc.GetPublishedEvents()).Returns(new List<object>());
            var mockObject = mock.Object;
            var handler = new RegisterOrganizationHandler(mockObject);
            var context = new TestableMessageHandlerContext();
            await handler.Handle(new RegisterOrganization(), context).ConfigureAwait(false);
        }
    }
}