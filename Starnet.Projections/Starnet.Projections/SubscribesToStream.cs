using System;

namespace Starnet.Projections
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class SubscribesToStream : Attribute
    {
        public string Name { get; private set; }
        public SubscribesToStream(string name)
        {
            Name = name;
        }
    }
}