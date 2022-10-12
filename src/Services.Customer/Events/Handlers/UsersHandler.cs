using System.Threading.Tasks;
using Services.Customer.Data;
using Services.Customer.Messages;
using Shared.Kafka.Consumer;

namespace Services.Customer.Handlers
{
    public class UsersHandler : IKafkaHandler<string, User>
    {
        private readonly CustomerDBContext _dbContext;

        public UsersHandler(CustomerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(string key, User value)
        {
            if (key == "Add")
            {

            _dbContext.Customers.Add(new Customer.Data.Customer
            {
                Id = value.Id,
                Email = value.Email,
                FirstName = value.FirstName,
                LastName = value.LastName,
            });

            await _dbContext.SaveChangesAsync();
            }
            if (key == "Update")
            {
                var user = await _dbContext.Customers.FindAsync(value.Id);
                user.Email = value.Email;
                user.FirstName = value.FirstName;
                user.LastName = value.LastName;
                //user.Password = value.Password;
                //user.Address=value.Address;
                _dbContext.Customers.Update(user);
                await _dbContext.SaveChangesAsync();
            }

            if (key == "Delete")
            {
            var user = await _dbContext.Customers.FindAsync(value.Id);


            _dbContext.Customers.Remove(user);


            await _dbContext.SaveChangesAsync();
            }


        }

    }
}