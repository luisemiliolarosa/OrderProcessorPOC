using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace OrderProcessor.Services
{
    public interface IFeatureFlagProvider
    {
        Task<bool> IsEnabledAsync(string feature);
    }

    public class KeyVaultFeatureFlagProvider : IFeatureFlagProvider
    {
        private readonly SecretClient _client;

        public KeyVaultFeatureFlagProvider(string keyVaultName)
        {
            var uri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            _client = new SecretClient(uri, new DefaultAzureCredential());
        }

        public async Task<bool> IsEnabledAsync(string feature)
        {
            try
            {
                var secret = await _client.GetSecretAsync(feature);
                if (secret?.Value?.Value != null && bool.TryParse(secret.Value.Value, out var v))
                    return v;
            }
            catch { }
            return false;
        }
    }
}
