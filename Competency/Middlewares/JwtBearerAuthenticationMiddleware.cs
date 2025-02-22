using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Competency.Interfaces;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace Competency.Middlewares;

public class JwtBearerAuthenticationMiddleware : AuthenticationHandler<JwtBearerOptions>
{
    private readonly IConfiguration configuration;
    private readonly IHashicorpVaultService vaultService;
    private readonly ILogger<JwtBearerAuthenticationMiddleware> log;

    public JwtBearerAuthenticationMiddleware(IHashicorpVaultService vaultService,ILogger<JwtBearerAuthenticationMiddleware> log, IConfiguration configuration, IOptionsMonitor<JwtBearerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock)
    : base(options, logger, encoder, clock)
    {
        this.configuration = configuration;
        this.log = log;
        this.vaultService=vaultService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return await Task.FromResult(AuthenticateResult.Fail("Authorization header missing"));
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (!authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));
            }
            var jwtToken = authHeader.Parameter;
            if (string.IsNullOrEmpty(jwtToken))
            {
                return AuthenticateResult.Fail("Token is missing.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = Options.TokenValidationParameters;
            validationParameters.IssuerSigningKey = await GetSigningKeyFromVaultServer();
            var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out SecurityToken securityToken);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail($"Authentication failed: {ex.Message}"));
        }
    }
    private async Task<RsaSecurityKey> GetSigningKeyFromVaultServer()
    {
        string vautlAppRoleToken = await vaultService.GetAppRoleTokenFromVault();
        var hashiCorpHttpClient = configuration["HashiCorp:HttpClient:BaseAddress"];
        if (string.IsNullOrEmpty(vautlAppRoleToken) || string.IsNullOrEmpty(hashiCorpHttpClient))
        {
            log.LogWarning("La configuration de HashiCorp Vault est manquante ou invalide.");
            throw new InvalidOperationException("La configuration de HashiCorp Vault est manquante ou invalide.");
        }
        var vaultClientSettings = new VaultClientSettings($"{hashiCorpHttpClient}", new TokenAuthMethodInfo(vautlAppRoleToken));
        var vaultClient = new VaultClient(vaultClientSettings);
        try
        {
            var secretPath = configuration["HashiCorp:JwtPublicKeyPath"];
            var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath);
            if (secret == null)
            {
                log.LogError("Le secret Vault est introuvable.");
                throw new InvalidOperationException("Le secret Vault est introuvable.");
            }
            var secretData = secret.Data.Data;
            if (!secretData.ContainsKey("authenticationSignatureKey"))
            {
                log.LogError("La clé publique 'authenticationSignatureKey' est manquante dans le secret Vault.");
                throw new InvalidOperationException("La clé publique 'authenticationSignatureKey' est introuvable.");
            }
            string rawPublicKeyPem = secretData["authenticationSignatureKey"].ToString()!;
            rawPublicKeyPem = rawPublicKeyPem.Trim();
            if (!rawPublicKeyPem.Contains("-----BEGIN RSA PUBLIC KEY-----") ||
                !rawPublicKeyPem.Contains("-----END RSA PUBLIC KEY-----"))
            {
                log.LogWarning("La clé récupérée n'a pas le bon format PEM.");
                throw new Exception("La clé récupérée n'a pas le bon format PEM.");
            }
            string keyBody = rawPublicKeyPem
                .Replace("-----BEGIN RSA PUBLIC KEY-----", "")
                .Replace("-----END RSA PUBLIC KEY-----", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Trim();
            if (string.IsNullOrEmpty(keyBody))
            {
                throw new Exception("Le contenu de la clé est vide après le nettoyage.");
            }
            string formattedPublicKeyPem = "-----BEGIN RSA PUBLIC KEY-----\n" +
                string.Join("\n", Enumerable.Range(0, (keyBody.Length + 63) / 64)
                    .Select(i => keyBody.Substring(i * 64, Math.Min(64, keyBody.Length - (i * 64))))) +
                "\n-----END RSA PUBLIC KEY-----";
            var rsa = RSA.Create();
            rsa.ImportFromPem(formattedPublicKeyPem);
            var rsaSecurityKey = new RsaSecurityKey(rsa);
            log.LogInformation("La clé publique a été récupérée et formatée avec succès.");
            return rsaSecurityKey;
        }
        catch (FormatException ex)
        {
            log.LogError(ex, "Erreur lors de la conversion de la clé publique Base64 ");
            throw new FormatException("Erreur lors de la conversion de la clé publique Base64.", ex);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Erreur lors de la récupération de la clé publique dans Vault");
            throw new Exception("Erreur lors de la récupération de la clé publique dans Vault", ex);
        }
    }
}