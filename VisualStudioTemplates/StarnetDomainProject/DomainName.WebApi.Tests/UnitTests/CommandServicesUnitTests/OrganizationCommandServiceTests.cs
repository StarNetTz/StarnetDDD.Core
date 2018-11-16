using NUnit.Framework;
using ServiceStack;
using $ext_projectname$.WebApi.ServiceInterface;
using $ext_projectname$.WebApi.ServiceModel;
using System.Threading.Tasks;
using ServiceStack.FluentValidation;
using System.Collections.Generic;
using ServiceStack.FluentValidation.Results;
using System.Linq;

namespace $safeprojectname$
{
    public class OrganizationCommandServiceTests : CommandServiceTestBase<OrganizationService>
    {
        [Test]
        public async Task can_execute_register_company()
        {
            var response = await Service.Any(new RegisterOrganization { Id = "Organizations-1", Name = "My company", Address = new Address { City = "Cardiff", Country = "UK", State = "Essex", Street = "Baker 223", ZipCode = "9876" } }) as ResponseStatus;
            Assert.That(response.Errors, Is.Null);
        }

        [Test]
        public void id_name_and_address_must_be_present()
        {
            var myValidator = AppHost.TryResolve<IValidator<RegisterOrganization>>();
            var result = myValidator.Validate(new RegisterOrganization());
            Assert.That(result.IsValid, Is.False);
            AssertPropertyInvalid(result.Errors, "Id", "NotEmpty");
            AssertPropertyInvalid(result.Errors, "Name", "NotEmpty");
            AssertPropertyInvalid(result.Errors, "Address", "NotEmpty");
        }

        void AssertPropertyInvalid(IList<ValidationFailure> errors, string property, string errorCode)
        {
            var item = (from e in errors where e.PropertyName == property && e.ErrorCode == errorCode select e).FirstOrDefault();
            Assert.That(item, Is.Not.Null);
        }
    }
}