using System.Security;
using System.Security.Claims;

namespace Bdcf.Extensions.Security;

public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Gets the claim specified by <paramref name="claimType"/> from the <paramref name="principal"/> and returns it as the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the claim value to return.</typeparam>
	/// <returns>The claim value, or the default of the value type.</returns>
	public static T? GetClaim<T>(this ClaimsPrincipal principal, string claimType)
	{
		string? currentVal = principal.FindFirst(claimType)?.Value;
		return currentVal is not null ? ConvertClaimValue<T>(currentVal, claimType, required: false) : default;
	}

	/// <summary>
	/// Gets the claim specified by <paramref name="claimType"/> from the <paramref name="principal"/> and returns it as a string.
	/// </summary>
	/// <param name="principal"></param>
	/// <param name="claimType"></param>
	/// <returns></returns>
	public static string? GetClaim(this ClaimsPrincipal principal, string claimType)
	{
		return principal.GetClaim<string>(claimType);
	}

	/// <summary>
	/// Gets the claim specified by <paramref name="claimType"/> from the <paramref name="principal"/> and returns it as the specified type.
	/// If the claim is not found, a <see cref="SecurityException"/> is thrown.
	/// </summary>
	/// <typeparam name="T">The type of the claim value to return.</typeparam>
	/// <returns>The claim value.</returns>
	public static T GetRequiredClaim<T>(this ClaimsPrincipal principal, string claimType)
	{
		string? currentVal = principal.FindFirst(claimType)?.Value ??
			throw new SecurityException($"Required claim {claimType} could not be found.");

		return ConvertClaimValue<T>(currentVal, claimType, required: true) ??
			throw new SecurityException($"Required claim {claimType} could not be converted to {typeof(T)}.");
	}

	/// <summary>
	/// Gets the claim specified by <paramref name="claimType"/> from the <paramref name="principal"/> and returns it as a string.
	/// If the claim is not found, a <see cref="SecurityException"/> is thrown.
	/// </summary>
	/// <returns>The claim value.</returns>
	public static string GetRequiredClaim(this ClaimsPrincipal principal, string claimType)
	{
		return principal.GetRequiredClaim<string>(claimType);
	}

	/// <summary>
	/// Sets the claim specified by <paramref name="claimType"/> to <paramref name="value"/> on the <paramref name="principal"/>.
	/// If the claim does not already exist, it will be added. Otherwise, the existing claim will be removed and re-added with the new value.
	/// </summary>
	public static void SetClaim(this ClaimsPrincipal principal, string claimType, object value)
	{
		var identity = principal.Identity as ClaimsIdentity;
		if (identity is not null)
		{
			var existingClaim = identity.FindFirst(claimType);
			if (existingClaim is not null)
			{
				identity.RemoveClaim(existingClaim);
			}
			identity.AddClaim(new Claim(claimType, value?.ToString() ?? string.Empty));
		}
	}

	/// <summary>
	/// Adds the claim specified by <paramref name="claimType"/> to <paramref name="value"/> on the <paramref name="principal"/>.
	/// If the claim already exists, it will be left unchanged.
	/// </summary>
	public static void AddClaim(this ClaimsPrincipal principal, string claimType, object value)
	{
		var identity = principal.Identity as ClaimsIdentity;
		if (identity is not null)
		{
			var existingClaim = identity.FindFirst(claimType);
			if (existingClaim is null)
			{
				identity.AddClaim(new Claim(claimType, value?.ToString() ?? string.Empty));
			}
		}
	}

	/// <summary>
	/// Removes the claim specified by <paramref name="claimType"/> from the <paramref name="principal"/>.
	/// </summary>
	public static void RemoveClaim(this ClaimsPrincipal principal, string claimType)
	{
		var identity = principal.Identity as ClaimsIdentity;
		if (identity is not null)
		{
			var existingClaim = identity.FindFirst(claimType);
			if (existingClaim is not null)
			{
				identity.RemoveClaim(existingClaim);
			}
		}
	}

	private static T? ConvertClaimValue<T>(string value, string claimType, bool required)
	{
		var targetType = typeof(T);

		if (targetType == typeof(string))
		{
			return (T)(object)value;
		}

		if (Nullable.GetUnderlyingType(targetType) is Type underlyingType)
		{
			object? convertedValue = Convert.ChangeType(value, underlyingType);
			return (T?)convertedValue;
		}

		try
		{
			return (T)Convert.ChangeType(value, typeof(T));
		}
		catch
		{
			if (required)
			{
				throw new SecurityException($"Required claim {claimType} could not be converted to {typeof(T).Name}.");
			}

			return default;
		}
	}
}
