namespace LAPS.Web.Lib.Models;

/// <summary>
/// Interface for holding computer account data from Active Directory
/// with the LAPS password and it's expiration date.
/// </summary>
public interface IComputerAccount
{
    /// <summary>
    /// The computer account name in Active Directory.
    /// </summary>
    string ComputerName { get; init; }

    /// <summary>
    /// The LAPS password for the computer account.
    /// </summary>
    string? ComputerAdminPassword { get; init; }

    /// <summary>
    /// The <see cref="DateTimeOffset"/> when the LAPS password expires.
    /// </summary>
    DateTimeOffset ComputerAdminPasswordExpirationDateTime { get; init; }
}