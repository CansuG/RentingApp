﻿using Microsoft.AspNetCore.Identity;
using Renting.Models.Account;

namespace Renting.Repository;

public interface IAccountRepository
{
    public Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken);

    public Task<ApplicationUserIdentity> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    public Task<ApplicationUserIdentity> GetByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken);

    public Task<ApplicationUserIdentity> GetByUserId(int applicationUserId, CancellationToken cancellationToken);

    public Task<IdentityResult> UpdateAsync(ApplicationUserIdentity user);

    public Task<List<string>> GetUsernamesAsync();

    public Task<List<string>> GetEmailsAsync();

    public Task<IdentityResult> UpdateProfilePhotoAsync(int applicationUserId, String publicId, String imageURL);
}
