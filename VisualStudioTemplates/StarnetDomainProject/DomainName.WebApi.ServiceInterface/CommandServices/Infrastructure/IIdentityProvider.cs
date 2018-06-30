using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface IIdentityProvider
    {
        Task<string> GetId(string aggregateName);
    }
}
