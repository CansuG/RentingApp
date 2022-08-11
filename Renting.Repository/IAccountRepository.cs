using Microsoft.AspNetCore.Identity;
using Renting.Models.Account;

namespace Renting.Repository
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken);

        public Task<ApplicationUserIdentity> GetByEmailAsync(string normalizedUsername, CancellationToken cancellationToken);
    }
}
