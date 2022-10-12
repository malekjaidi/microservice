
using System;
using MediatR;

namespace Services.Identity.Commands
{
    public class RemoveUserCommand : IRequest
    {
        public Guid Id { get; set; }

    }
}