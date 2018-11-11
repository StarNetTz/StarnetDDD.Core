using System;

namespace $safeprojectname$.Commands
{
    public interface ICommand
    {
        string Id { get; }
    }

    public abstract class Command : ICommand
    {
        public string IssuedBy { get; set; }
        public DateTime TimeIssued { get; set; }

        public string Id { get; set; }
    }
}