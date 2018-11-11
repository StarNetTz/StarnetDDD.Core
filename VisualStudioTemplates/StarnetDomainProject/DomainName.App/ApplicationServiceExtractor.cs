using $ext_projectname$.Domain.Organization;
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
            Assembly assembly = Assembly.GetAssembly(typeof(OrganizationInteractor));
            return assembly.GetTypes().Where(p => typeof(IInteractor).IsAssignableFrom(p) && p.IsClass).ToList();
        }
    }
}
