using $ext_projectname$.WebApi.ServiceModel;
using ServiceStack;
using ServiceStack.FluentValidation;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class CompanyService : DomainCommandService
    {
        public async Task<object> Any(RegisterCompany request)
        {
            return await TryProcessRequest<PL.Commands.RegisterCompany>(request);
        }
    }

    public class RegisterCompanyValidator : AbstractValidator<RegisterCompany>
    {
        public RegisterCompanyValidator()
        {
            RuleFor(c => c.Id).NotEmpty().Matches("Companies-\\w");
            RuleFor(c => c.Name).NotEmpty().Length(2, 150);
            RuleFor(c => c.Address).NotEmpty().SetValidator(new AddressValidator());
        }
    }
}
