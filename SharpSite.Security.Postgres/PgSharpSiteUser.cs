using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharpSite.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Security.Postgres;

public class PgSharpSiteUser : IdentityUser
{

	[PersonalData, Required, MaxLength(50)]
	public required string DisplayName { get; set; }

	public static explicit operator SharpSiteUser(PgSharpSiteUser user) =>
			new(user.Id, user.UserName, user.Email)
			{
				DisplayName = user.DisplayName
			};

	public static explicit operator PgSharpSiteUser(SharpSiteUser user) =>
		new()
		{
			Id = user.Id,
			DisplayName = user.DisplayName,
			UserName = user.UserName,
			Email = user.Email
		};

}

public class PgSecurityContext(DbContextOptions<PgSecurityContext> options) : IdentityDbContext<PgSharpSiteUser>(options)
{
}

public class PgUserManager(IUserStore<PgSharpSiteUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<PgSharpSiteUser> passwordHasher, IEnumerable<IUserValidator<PgSharpSiteUser>> userValidators, IEnumerable<IPasswordValidator<PgSharpSiteUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<PgSharpSiteUser>> logger): UserManager<PgSharpSiteUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
	private readonly ILogger<UserManager<PgSharpSiteUser>> _logger = logger;

	protected override string CreateTwoFactorRecoveryCode()
	{
		return base.CreateTwoFactorRecoveryCode();
	}

	protected override Task<IdentityResult> UpdatePasswordHash(PgSharpSiteUser user, string newPassword, bool validatePassword)
	{
		return base.UpdatePasswordHash(user, newPassword, validatePassword);
	}

	protected override Task<IdentityResult> UpdateUserAsync(PgSharpSiteUser user)
	{
		return base.UpdateUserAsync(user);
	}

	protected override async Task<PasswordVerificationResult> VerifyPasswordAsync(IUserPasswordStore<PgSharpSiteUser> store, PgSharpSiteUser user, string password)
	{
		var result = await base.VerifyPasswordAsync(store, user, password);
		_logger.LogInformation("User {UserId} password verification completed with {PasswordVerificationResult}", user.Id, result);
		return result;
	}
}