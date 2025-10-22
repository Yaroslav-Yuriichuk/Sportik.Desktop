using Windows.Security.Credentials;
using Newtonsoft.Json;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class SecureCacheService : ISecureCacheService
    {
        private const string ResourceName = "Sportik Desktop";

        private readonly PasswordVault _vault = new PasswordVault();

        public T Get<T>()
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";

            PasswordCredential credential = _vault.Retrieve(ResourceName, key);
            credential.RetrievePassword();

            return JsonConvert.DeserializeObject<T>(credential.Password);
        }

        public bool TryGet<T>(out T value)
        {
            value = default;
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";

            try
            {
                PasswordCredential credential = _vault.Retrieve(ResourceName, key);
                credential.RetrievePassword();

                value = JsonConvert.DeserializeObject<T>(credential.Password);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Set<T>(T value)
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";
            string json = JsonConvert.SerializeObject(value);

            try
            {
                PasswordCredential credential = _vault.Retrieve(ResourceName, key);
                _vault.Remove(credential);
            }
            catch
            {
                // Ignored.
            }

            _vault.Add(new PasswordCredential(ResourceName, key, json));
        }

        public void Remove<T>()
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";

            try
            {
                PasswordCredential credential = _vault.Retrieve(ResourceName, key);
                _vault.Remove(credential);
            }
            catch
            {
                // Ignored.
            }
        }
    }
}