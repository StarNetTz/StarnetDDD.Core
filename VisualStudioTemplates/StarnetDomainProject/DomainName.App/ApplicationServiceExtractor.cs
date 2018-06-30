using $ext_projectname$.Domain.Company;
using Starnet.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace $safeprojectname$
{
    public class ApplicationServiceExtractor
    {
        public static List<Type> GetApplicationServiceClassTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(CompanyApplicationService));
            return assembly.GetTypes().Where(p => typeof(IApplicationService).IsAssignableFrom(p) && p.IsClass).ToList();
        }
    }
}
