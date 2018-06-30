using NUnit.Framework;
using $ext_projectname$.PL.Commands;
using $ext_projectname$.PL.Events;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$.CompanyTests
{
    class OpenCompanyTests : _ServiceSpec
    {
        RegisterCompany RegisterCompanyCommand;
        CompanyRegistered CompanyRegisteredEvent;

        protected string AggregateId = "companies-1";

        [SetUp]
        public void Setup()
        {
            RegisterCompanyCommand = new RegisterCompany() { Id = AggregateId, IssuedBy = "zeko", Name = "Xamics Ltd", TimeIssued = DateTime.MinValue, Address = new PL.Address { Street = "321 Bakers Street b", City="London", Country="UK", State="Essex", ZipCode="3021" }, VATId = "754" };
            CompanyRegisteredEvent = new CompanyRegistered() { Id = AggregateId, IssuedBy = "zeko", Name = "Xamics Ltd", TimeIssued = DateTime.MinValue, Address = new PL.Address { Street = "321 Bakers Street b", City = "London", Country = "UK", State = "Essex", ZipCode = "3021" }, VATId = "754" };
        }

        [Test]
        public async Task can_register_company()
        {
            Given();
            When(RegisterCompanyCommand);
            await Expect(CompanyRegisteredEvent);
        }

        [Test]
        public async Task cannot_register_company_that_is_already_registered()
        {
            Given(CompanyRegisteredEvent);
            When(RegisterCompanyCommand);
            await ExpectError("CompanyAlreadyExists");
        }
    }
}