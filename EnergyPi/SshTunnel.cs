using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace EnergyPi;

public class SshSettings
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int SshPort { get; set; }

    public string DatabaseHost { get; set; }
    public int DatabasePort { get; set; }
    public string BoundHost { get; set; }
    public int BoundPort { get; set; }
}

public class SshTunnel
{
    private readonly SshSettings? _settings;
    private SshClient? sshClient = null;
    private uint? localPort = null;

    public SshTunnel(IConfiguration configuration)
    {
        _settings = configuration.GetRequiredSection("SshSettings").Get<SshSettings>();
        Reconnect();
    }

    private void Reconnect()
    {
        (sshClient, localPort) =
            ConnectSsh(_settings.HostName, _settings.UserName, _settings.Password, sshPort: _settings.SshPort, boundHost: _settings.BoundHost, boundPort: _settings.BoundPort);
        sshClient.ErrorOccurred += SshClient_ErrorOccurred;
    }

    private void SshClient_ErrorOccurred(object? sender, Renci.SshNet.Common.ExceptionEventArgs e)
    {
        Console.WriteLine($"ERROR: SSHCLIENT {e.Exception.Message}");
        Console.WriteLine(e.Exception);
    }

    private bool IsConnected => sshClient is { IsConnected: true };

    private static (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null,
        string sshKeyFile = null, string sshPassPhrase = null, int sshPort = 22, string databaseServer = "localhost", int databasePort = 3306, int boundPort = 3306, string boundHost = "127.0.0.1")
    {
        // check arguments
        if (string.IsNullOrEmpty(sshHostName))
            throw new ArgumentException($"{nameof(sshHostName)} must be specified.", nameof(sshHostName));
        if (string.IsNullOrEmpty(sshHostName))
            throw new ArgumentException($"{nameof(sshUserName)} must be specified.", nameof(sshUserName));
        if (string.IsNullOrEmpty(sshPassword) && string.IsNullOrEmpty(sshKeyFile))
            throw new ArgumentException($"One of {nameof(sshPassword)} and {nameof(sshKeyFile)} must be specified.");
        if (string.IsNullOrEmpty(databaseServer))
            throw new ArgumentException($"{nameof(databaseServer)} must be specified.", nameof(databaseServer));

        // define the authentication methods to use (in order)
        var authenticationMethods = new List<AuthenticationMethod>();
        if (!string.IsNullOrEmpty(sshKeyFile))
        {
            authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshUserName,
                new PrivateKeyFile(sshKeyFile, string.IsNullOrEmpty(sshPassPhrase) ? null : sshPassPhrase)));
        }
        if (!string.IsNullOrEmpty(sshPassword))
        {
            authenticationMethods.Add(new PasswordAuthenticationMethod(sshUserName, sshPassword));
        }

        // connect to the SSH server
        var sshClient = new SshClient(new ConnectionInfo(sshHostName, sshPort, sshUserName, authenticationMethods.ToArray()));
        sshClient.Connect();

        // forward a local port to the database server and port, using the SSH server
        var forwardedPort = new ForwardedPortLocal(boundHost, (uint)boundPort, databaseServer, (uint)databasePort);
        sshClient.AddForwardedPort(forwardedPort);
        forwardedPort.Start();

        return (sshClient, forwardedPort.BoundPort);
    }

}