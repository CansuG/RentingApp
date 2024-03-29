﻿namespace Renting.Models.Account;

public class ApplicationUserIdentity
{
    public int ApplicationUserId { get; set; }

    public string Username { get; set; }

    public string NormalizedUsername { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public string Gender { get; set; }

    public string PasswordHash { get; set; }

    public string? FirstName { get; set;}

    public string? LastName { get; set;}

    public string ImageUrl { get; set; }

    public string PublicId { get; set; }
}
