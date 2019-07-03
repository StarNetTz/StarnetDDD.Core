using System;
using System.Collections.Generic;
using System.Text;

namespace Starnet.Aggregates.Tests.Domain.PL.Commands
{
    public class CreatePerson : ICommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RenamePerson : ICommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
