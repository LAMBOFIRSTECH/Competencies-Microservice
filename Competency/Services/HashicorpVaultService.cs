using System.Net.Sockets;
using Competency.Interfaces;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
namespace Competency.Services;
public class HashicorpVaultService : IHashicorpVaultService
{
    private readonly IConfiguration configuration;
    private readonly ILogger<HashicorpVaultService> log;
    public HashicorpVaultService(IConfiguration configuration, ILogger<HashicorpVaultService> log)
    {
        this.configuration = configuration;
        this.log = log;
    }
    public async Task<string> GetAppRoleTokenFromVault()
    {
        var hashiCorpRoleID = configuration["HashiCorp:AppRole:RoleID"];
        var hashiCorpSecretID = configuration["HashiCorp:AppRole:SecretID"];
        var hashiCorpHttpClient = configuration["HashiCorp:HttpClient:BaseAddress"];
        if (string.IsNullOrEmpty(hashiCorpRoleID) || string.IsNullOrEmpty(hashiCorpSecretID) || string.IsNullOrEmpty(hashiCorpHttpClient))
        {
            log.LogWarning("Empty or invalid HashiCorp Vault configurations.");
            throw new InvalidOperationException("Empty or invalid HashiCorp Vault configurations.");
        }
        var appRoleAuthMethodInfo = new AppRoleAuthMethodInfo(hashiCorpRoleID, hashiCorpSecretID);
        var vaultClientSettings = new VaultClientSettings($"{hashiCorpHttpClient}", appRoleAuthMethodInfo);
        var vaultClient = new VaultClient(vaultClientSettings);
        try
        {
            var authResponse = await vaultClient.V1.Auth.AppRole.LoginAsync(appRoleAuthMethodInfo);
            string token = authResponse.AuthInfo.ClientToken;
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Empty token retrieve from HashiCorp Vault");
            return token;
        }
        catch (Exception ex) when (ex.InnerException is SocketException socket)
        {
            log.LogError(socket, "Socket's problems check if Hashicorp Vault server is UP", socket.Message);
            throw new InvalidOperationException("The service is unavailable. Please retry soon.", ex);
        }
    }
    public async Task<string> GetRabbitConnectionStringFromVault()
    {
        string vautlAppRoleToken = await GetAppRoleTokenFromVault();
        var secretPath = configuration["HashiCorp:RabbitMqPath"];
        var hashiCorpHttpClient = configuration["HashiCorp:HttpClient:BaseAddress"];
        if (string.IsNullOrEmpty(hashiCorpHttpClient) || string.IsNullOrEmpty(secretPath))
        {
            log.LogWarning("Empty or invalid HashiCorp Vault configurations.");
            throw new InvalidOperationException("Empty or invalid HashiCorp Vault configurations.");
        }
        var vaultClientSettings = new VaultClientSettings($"{hashiCorpHttpClient}", new TokenAuthMethodInfo(vautlAppRoleToken));
        var vaultClient = new VaultClient(vaultClientSettings);
        var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath);
        if (secret == null)
        {
            log.LogError("Le secret Vault est introuvable pour la chaine de connection rabbitMQ.");
            throw new InvalidOperationException("Le secret Vault est introuvable.");
        }
        var secretData = secret.Data.Data;
        if (!secretData.ContainsKey("rabbitMqConnectionString"))
        {
            log.LogError("La clé 'rabbitMqConnectionString' est manquante dans le secret Vault.");
            throw new InvalidOperationException("La clé 'rabbitMqConnectionString' est introuvable.");
        }
        return secretData["rabbitMqConnectionString"].ToString()!;
    }
}