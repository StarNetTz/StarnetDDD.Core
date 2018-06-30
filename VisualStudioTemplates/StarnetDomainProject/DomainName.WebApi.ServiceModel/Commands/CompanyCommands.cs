using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace $safeprojectname$
{
    [Route("/companies", Verbs = "POST")]
    public class RegisterCompany : IReturn<ResponseStatus>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public string VATId { get; set; }
    }
}
