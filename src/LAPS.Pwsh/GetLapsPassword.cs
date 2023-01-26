using System.Management.Automation;
using LAPS.Lib.Models;
// ReSharper disable MemberCanBePrivate.Global

namespace LAPS.Pwsh;

[Cmdlet(VerbsCommon.Get, "LapsPassword")]
public class GetLapsPassword: PSCmdlet
{
    [Parameter(Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string ComputerName { get; set; } = null!;

    [Parameter(Position = 1, Mandatory = true)]
    public PSCredential Credentials { get; set; } = null!;

    [Parameter(Position = 2, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string DomainName { get; set; } = null!;

    [Parameter(Position = 3)]
    public string? ServerName { get; set; }


    protected override void ProcessRecord()
    {
        ComputerAccount computerAccount = ComputerAccountSearcher.GetComputerAccount(
            computerName: ComputerName,
            domainName: DomainName,
            serverName: ServerName!,
            username: Credentials.UserName,
            password: Credentials.GetNetworkCredential().Password
        );

        WriteObject(computerAccount);
    }
}