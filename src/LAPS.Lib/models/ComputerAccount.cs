using System.Text.Json.Serialization;

#if IsWindows
using System.DirectoryServices;
#endif

namespace LAPS.Lib.Models;

/// <summary>
/// Holds data about a computer account in Active Directory and data related to LAPS.
/// </summary>
public class ComputerAccount: JsonOutItem<ComputerAccount>, IComputerAccount
{
    [JsonConstructor]
    public ComputerAccount()
    {}

#pragma warning disable CA1416
#if IsWindows
    /// <summary>
    /// Create from <see cref="ResultPropertyCollection"/> on a <see cref="SearchResult"/> returned by a <see cref="DirectorySearcher"/>.
    /// </summary>
    /// <param name="properties">The attributes of a computer account in Active Directory.</param>
    /// <exception cref="NullReferenceException">Thrown when the 'name' attribute is null.</exception>
    public ComputerAccount(ResultPropertyCollection properties)
    {
        // If the 'name' attribute is null,
        // throw a NullReferenceException.
        if (properties["name"][0].ToString() is null)
        {
            throw new NullReferenceException("Unable to resolve the 'name' attribute from the supplied input");
        }

        ComputerName = properties["name"][0].ToString()!;
        ComputerAdminPassword = properties["ms-mcs-admpwd"][0].ToString();

        // If the 'ms-mcs-admpwdexpirationtime' attribute is not null, convert it from the the value.
        // Otherwise, set it to the minimum value for DateTimeOffset.
        ComputerAdminPasswordExpirationDateTime = properties["ms-mcs-admpwdexpirationtime"][0].ToString() is null
            ? DateTimeOffset.MinValue
            : DateTimeOffset.FromFileTime(Convert.ToInt64(properties["ms-mcs-admpwdexpirationtime"][0].ToString()));
    }
#endif
#pragma warning restore CA1416

    /// <inheritdoc />
    [JsonPropertyName("computerName")]
    public string ComputerName { get; init; } = null!;

    /// <inheritdoc />
    [JsonPropertyName("computerAdminPassword")]
    public string? ComputerAdminPassword { get; init; }

    /// <inheritdoc />
    [JsonPropertyName("computerAdminPasswordExpirationDateTime")]
    public DateTimeOffset ComputerAdminPasswordExpirationDateTime { get; init; }
}