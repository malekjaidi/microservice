
using System;
using MediatR;

namespace Services.Identity.Commands
{
    public class UpdateUserCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
  
    }
}