namespace KubernetesVolume.Addon.Clients
{
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using RestSharp.Authenticators;

    internal class KubernetesClient : BaseRestClient
    {
        private readonly string @namespace;

        private KubernetesClient(string baseUrl, string @namespace)
            : base(baseUrl)
        {
            this.@namespace = @namespace;
        }

        internal static KubernetesClient FromUsername(string baseUrl, string @namespace, string username, string password)
        {
            return new KubernetesClient(baseUrl, @namespace)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
        }

        internal static KubernetesClient FromCertificate(string baseUrl, string @namespace, X509Certificate2 certificate)
        {
            return new KubernetesClient(baseUrl, @namespace)
            {
                ClientCertificates = new X509Certificate2Collection { certificate }
            };
        }

        internal async Task<IRestResponse<dynamic>> PostPersistentVolumeClaimAsync(object claim)
        {
            return await ExecRequestAsync<object>(Method.POST, $"/api/v1/namespaces/{@namespace}/persistentvolumeclaims", JObject.FromObject(claim));
        }

        internal async Task<IRestResponse<dynamic>> GetPersistentVolumeClaimAsync(string name)
        {
            return await ExecRequestAsync<object>(Method.GET, $"/api/v1/namespaces/{@namespace}/persistentvolumeclaims/{name}");
        }

        internal async Task<IRestResponse<dynamic>> DeletePersistentVolumeClaimAsync(string name)
        {
            return await ExecRequestAsync<object>(Method.DELETE, $"/api/v1/namespaces/{@namespace}/persistentvolumeclaims/{name}");
        }
    }
}
