using MBAM.Annotations.Domain.Core.Events;
using MediatR;
using System;

namespace MBAM.Annotations.Domain.Core.Commands
{
    public class Command : Message, IRequest
    {
        public DateTime TimeStamp { get; private set; }

        public Command()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
