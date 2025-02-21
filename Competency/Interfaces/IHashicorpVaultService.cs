namespace Competency.Interfaces;
public interface IHashicorpVaultService
{
    Task<string> GetRabbitConnectionStringFromVault();
    Task<string> GetAppRoleTokenFromVault();
}