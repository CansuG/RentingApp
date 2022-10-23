using Microsoft.Extensions.Configuration;
using Renting.Models.Account;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace Renting.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IConfiguration _config;

    public AccountRepository(IConfiguration config)
    {
        _config = config;
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dataTable = new DataTable();
        dataTable.Columns.Add("Username", typeof(string));
        dataTable.Columns.Add("NormalizedUsername", typeof(string));
        dataTable.Columns.Add("Email", typeof(string));
        dataTable.Columns.Add("NormalizedEmail", typeof(string));
        dataTable.Columns.Add("Gender", typeof(string));
        dataTable.Columns.Add("PasswordHash", typeof(string));
        dataTable.Columns.Add("FirstName", typeof(string));
        dataTable.Columns.Add("LastName", typeof(string));

        dataTable.Rows.Add(
            user.Username,
            user.NormalizedUsername,
            user.Email,
            user.NormalizedEmail,
            user.Gender,
            user.PasswordHash,
            user.FirstName,
            user.LastName
            );

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(
                "Account_Insert",
                new { Account = dataTable.AsTableValuedParameter("dbo.AccountType") }, 
                commandType: CommandType.StoredProcedure);
        }

        return IdentityResult.Success;

    }

    public async Task<ApplicationUserIdentity> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUserIdentity applicationUser;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync(cancellationToken);

            applicationUser = await connection.QuerySingleOrDefaultAsync<ApplicationUserIdentity>(
                "Account_GetByEmail", 
                new { NormalizedEmail = normalizedEmail },
                commandType: CommandType.StoredProcedure
                );
        }

        return applicationUser;
    }

    public async Task<ApplicationUserIdentity> GetByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUserIdentity applicationUser;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync(cancellationToken);

            applicationUser = await connection.QuerySingleOrDefaultAsync<ApplicationUserIdentity>(
                "Account_GetByUsername", new { NormalizedUsername = normalizedUsername },
                commandType: CommandType.StoredProcedure
                );
        }

        return applicationUser;
    }

    public async Task<ApplicationUserIdentity> GetByUserId(int applicationUserId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUserIdentity applicationUser;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync(cancellationToken);

            applicationUser = await connection.QuerySingleOrDefaultAsync<ApplicationUserIdentity>(
                "Account_GetByUserId", new { ApplicationUserId = applicationUserId },
                commandType: CommandType.StoredProcedure
                );
        }
        return applicationUser;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUserIdentity user)
    {

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();


            await connection.ExecuteScalarAsync<int?>(
                "Account_Update",
                new { ApplicationUserId = user.ApplicationUserId,
                      Username = user.Username,
                      NormalizedUsername = user.NormalizedUsername,
                      Gender = user.Gender,
                      FirstName = user.FirstName,
                      LastName = user.LastName,
                },
                commandType: CommandType.StoredProcedure
                );
        }

        return IdentityResult.Success;

    }

    public async Task<List<string>> GetEmailsAsync()
    {
        IEnumerable<string> emails;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            emails = await connection.QueryAsync<string>(
                "Account_GetEmails",
                new {},
                commandType: CommandType.StoredProcedure);
        }

        return emails.ToList();
    }

    public async Task<List<string>> GetUsernamesAsync()
    {
        IEnumerable<string> usernames;

        using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();

            usernames = await connection.QueryAsync<string>(
                "Account_GetUsernames",
                new {},
                commandType: CommandType.StoredProcedure);
        }

        return usernames.ToList();
    }
    
}

