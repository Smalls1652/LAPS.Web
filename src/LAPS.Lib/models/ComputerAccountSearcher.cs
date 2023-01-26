#pragma warning disable CA1416
#if IsWindows
/*
 * Note:
 * This static class only compiles on Windows-based systems.
 */

using System.DirectoryServices;
using System.Text;

namespace LAPS.Lib.Models;

/// <summary>
/// Houses methods for getting <see cref="ComputerAccount"/> items from Active Directory.
/// </summary>
public static class ComputerAccountSearcher
{
    private static readonly string[] _propertiesToGet = { "name", "ms-mcs-admpwd", "ms-mcs-admpwdexpirationtime" };

    /// <summary>
    /// Get LAPS data for a single computer account in Active Directory.
    /// </summary>
    /// <param name="computerName">The name of the computer account.</param>
    /// <param name="domainName">The FQDN of the Active Directory forest.</param>
    /// <param name="serverName">The FQDN of the Active Directory domain controller to pull data from.</param>
    /// <param name="username">The username of the user to run the search as.</param>
    /// <param name="password">The password of the user to run the search as.</param>
    /// <returns>A single <see cref="ComputerAccount"/> item.</returns>
    /// <exception cref="Exception">Thrown when a computer account for the supplied name was not found.</exception>
    public static ComputerAccount GetComputerAccount(string computerName, string? domainName, string? serverName, string username, string password)
    {
        string ldapQuery = $"(&(objectClass=computer)((name={computerName})))";
        string ldapPath = CreateLdapPathString(domainName, serverName);

        using DirectoryEntry directoryEntry = CreateDirectoryEntry(ldapPath, username, password);

        using DirectorySearcher directorySearcher = new(
            searchRoot: directoryEntry,
            filter: ldapQuery,
            propertiesToLoad: _propertiesToGet
        );

        SearchResult? directorySearchResult = directorySearcher.FindOne();

        if (directorySearchResult is not null)
        {
            return new(directorySearchResult.Properties);
        }
        else
        {
            throw new Exception($"Could not find a computer object for '{computerName}'.");
        }
    }

    /// <summary>
    /// Get LAPS data for all computer accounts in Active directory.
    /// </summary>
    /// <param name="domainName">The FQDN of the Active Directory forest.</param>
    /// <param name="serverName">The FQDN of the Active Directory domain controller to pull data from.</param>
    /// <param name="username">The username of the user to run the search as.</param>
    /// <param name="password">The password of the user to run the search as.</param>
    /// <returns>A collection of <see cref="ComputerAccount"/> items.</returns>
    public static IEnumerable<ComputerAccount> GetComputerAccounts(string? domainName, string? serverName, string? username, string? password)
    {
        string ldapQuery = "(&(objectClass=computer)((ms-mcs-admpwd=*)))";
        string ldapPath = CreateLdapPathString(domainName, serverName);

        using DirectoryEntry directoryEntry = CreateDirectoryEntry(ldapPath, username, password);

        using DirectorySearcher directorySearcher = new(
            searchRoot: directoryEntry,
            filter: ldapQuery,
            propertiesToLoad: _propertiesToGet
        );

        SearchResultCollection directorySearchResults = directorySearcher.FindAll();

        ComputerAccount[] computerAccounts = new ComputerAccount[directorySearchResults.Count];
        for (int i = 0; i < directorySearchResults.Count; i++)
        {
            computerAccounts[i] = new(directorySearchResults[i].Properties);
        }

        return computerAccounts;
    }

    /// <summary>
    /// Create a LDAP path string to use with <see cref="DirectoryEntry"/>.
    /// </summary>
    /// <param name="domainName">The FQDN of the Active Directory forest.</param>
    /// <param name="serverName">The FQDN of the Active Directory domain controller to pull data from.</param>
    /// <returns>The LDAP path for the given domain.</returns>
    private static string CreateLdapPathString(string domainName, string? serverName)
    {
        StringBuilder ldapPathStringBuilder = new("LDAP://");

        if (serverName is not null || string.IsNullOrEmpty(serverName))
        {
            ldapPathStringBuilder.Append($"{serverName}/");
        }

        string[] domainNameSplit = domainName.Split(".");

        for (int i = 0; i < domainNameSplit.Length; i++)
        {
            ldapPathStringBuilder.Append($"DC={domainNameSplit[i]}");

            if (i != domainNameSplit.Length - 1)
            {
                ldapPathStringBuilder.Append(",");
            }
        }

        return ldapPathStringBuilder.ToString();
    }

    /// <summary>
    /// Create a <see cref="DirectoryEntry"/> instance.
    /// </summary>
    /// <param name="ldapPath">The LDAP path to use.</param>
    /// <param name="username">The username of the user to run the search as.</param>
    /// <param name="password">The password of the user to run the search as.</param>
    /// <returns>An instance of <see cref="DirectoryEntry"/>.</returns>
    private static DirectoryEntry CreateDirectoryEntry(string ldapPath, string? username, string? password)
    {
        return (username is not null && password is not null) switch
        {
            true => new DirectoryEntry(
                path: ldapPath,
                username: username,
                password: password
            ),
            _ => new DirectoryEntry(
                path: ldapPath
            )
        };
    }
}
#endif
#pragma warning restore CA1416