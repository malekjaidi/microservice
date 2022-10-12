using System.Threading.Tasks;
using System;
using System.Threading;
using Services.Identity.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Kafka;

namespace Services.Identity.Commands.Handlers
{
    public class UpdateuserCOmmandHandler : AsyncRequestHandler<UpdateUserCommand>
    {
        private readonly IdentityDBContext _dbContext;
        private readonly IKafkaMessageBus<string, User> _bus;

        public UpdateuserCOmmandHandler(IdentityDBContext dbContext, IKafkaMessageBus<string, User> bus)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        protected override async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.Id);

                user.Password = command.Password;
                user.Email = command.Email;
                user.FirstName = command.FirstName;
                user.LastName = command.LastName;
                user.Address = command.Address;


            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();

            await _bus.PublishAsync("Update", user);
        }
    }
}
