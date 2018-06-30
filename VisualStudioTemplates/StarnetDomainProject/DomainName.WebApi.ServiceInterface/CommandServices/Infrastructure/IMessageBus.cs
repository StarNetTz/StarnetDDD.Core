using System;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface IMessageBus
    {
        Task Send(object message);
    }
}
