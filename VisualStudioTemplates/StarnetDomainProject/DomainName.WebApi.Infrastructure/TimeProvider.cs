using $ext_projectname$.WebApi.ServiceInterface;
using System;

namespace $safeprojectname$
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime GetUtcTime()
        {
            return DateTime.UtcNow;
        }
    }
}
