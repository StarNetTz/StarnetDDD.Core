using System;
using ServiceStack;
using $ext_projectname$.WebApi.ServiceModel;

namespace $safeprojectname$
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }
    }
}
