using System;

namespace $safeprojectname$.Events
{
    public interface IEvent
    {
        string Id { get; }
    }

    public abstract class Event : IEvent
    {
        public string Id { get; set; }
        public string IssuedBy { get; set; }
        public DateTime TimeIssued { get; set; }
    }
}