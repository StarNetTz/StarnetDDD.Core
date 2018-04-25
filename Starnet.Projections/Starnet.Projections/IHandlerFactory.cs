using System;

namespace Starnet.Projections
{
    public interface IHandlerFactory
    {
        IHandler Create(Type t);
    }
}
