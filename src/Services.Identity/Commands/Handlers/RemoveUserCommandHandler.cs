using System.Threading.Tasks;
using System;
using System.Threading;
using Services.Identity.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Kafka;

namespace Services.Identity.Commands.Handlers
{
    public class RemoveUserCommandHandler : AsyncRequestHandler<RemoveUserCommand>
    {
        private readonly IdentityDBContext _dbContext;
        private readonly IKafkaMessageBus<string, User> _bus;

        public RemoveUserCommandHandler(IdentityDBContext dbContext, IKafkaMessageBus<string, User> bus)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        protected override async Task Handle(RemoveUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.Id);

            await _bus.PublishAsync("Delete", user);

            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

        }
    }
}
