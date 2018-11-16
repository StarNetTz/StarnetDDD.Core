using NUnit.Framework;
using $ext_projectname$.PL.Commands;
using $ext_projectname$.PL.Events;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$.OrganizationTests
{
    class RegisterOrganizationTests : _ServiceSpec
    {

        [Test]
        public async Task can_register_organization()
        {
            string aggId = "Organizations-1";
            Given();
            When(CreateRegisterOrganizationCommand(aggId));
            await Expect(CreateOrganizationRegistered(aggId));
        }

        [Test]
        public async Task cannot_register_company_that_is_already_registered()
        {
            string aggId = "Organizations-1";
            var cmd = CreateRegisterOrganizationCommand(aggId);
            var evt = CreateOrganizationRegistered(aggId);

            Given(evt);
            When(cmd);
            await ExpectError("OrganizationAlreadyExists");
        }

        RegisterOrganization CreateRegisterOrganizationCommand(string id)
        {
           return new RegisterOrganization() {
               Id = id,
               IssuedBy = "zeko",
               Name = "Xamics Ltd",
               TimeIssued = DateTime.MinValue,
               Address = new PL.Address {
                   Street = "321 Bakers Street b",
                   City = "London",
                   Country = "UK",
                   State = "Essex",
                   ZipCode = "3021" }
           };
        }

        OrganizationRegistered CreateOrganizationRegistered(string id)
        {
            return new OrganizationRegistered()
            {
                Id = id,
                IssuedBy = "zeko",
                Name = "Xamics Ltd",
                TimeIssued = DateTime.MinValue,
                Address = new PL.Address
                {
                    Street = "321 Bakers Street b",
                    City = "London",
                    Country = "UK",
                    State = "Essex",
                    ZipCode = "3021"
                }
            };
        }
    }
}