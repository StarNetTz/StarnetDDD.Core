using $ext_projectname$.WebApi.ServiceModel;
using ServiceStack.FluentValidation;
using System;

namespace $safeprojectname$
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(c => c.Street).NotEmpty().NotEmpty().Length(2, 150);
            RuleFor(c => c.City).NotEmpty().Length(2, 150);
            RuleFor(c => c.State).NotEmpty().Length(2, 150);
            RuleFor(c => c.ZipCode).NotEmpty().Length(2, 150);
            RuleFor(c => c.Country).NotEmpty().Length(2, 150);
        }
    }
}
