namespace KubernetesVolume.Addon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Apprenda.SaaSGrid.Addons;
    using Clients;
    using Extensions;

    internal class Logic
    {
        private readonly KubernetesClient client;
        private readonly string storageClass;

        internal Logic(AddonManifest manifest)
        {
            var @namespace = manifest.Properties.First(p => p.Key == "Namespace").Value;
            var certificateFile = manifest.Properties.First(p => p.Key == "Certificate").Value;
            var certificatePassword = manifest.Properties.First(p => p.Key == "Password").Value;

            storageClass = manifest.Properties.First(p => p.Key == "StorageClass").Value;

            if (!string.IsNullOrWhiteSpace(manifest.ProvisioningUsername) &&
                string.IsNullOrWhiteSpace(certificateFile))
            {
                client = KubernetesClient.FromUsername(
                    manifest.ProvisioningLocation,
                    @namespace,
                    manifest.ProvisioningUsername,
                    manifest.ProvisioningPassword);
            }

            if (!string.IsNullOrWhiteSpace(certificateFile) &&
                string.IsNullOrWhiteSpace(manifest.ProvisioningUsername))
            {
                var certificate = new X509Certificate2();
                var assemblyFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);

                certificate.Import(
                    Path.Combine(assemblyFolder, certificateFile),
                    certificatePassword.ToSecureString(),
                    X509KeyStorageFlags.DefaultKeySet);
                client = KubernetesClient.FromCertificate(
                    manifest.ProvisioningLocation,
                    @namespace,
                    certificate);
            }

            if (client == null)
            {
                throw new Exception("Specify either a username or a certificate for authentication.");
            }
        }

        internal async Task ProvisionVolume(string name, string size)
        {
            await client.PostPersistentVolumeClaimAsync(new
            {
                apiVersion = "v1",
                kind = "PersistentVolumeClaim",
                metadata = new
                {
                    name,
                    annotations = new Dictionary<string, string>
                    {
                        {  "volume.beta.kubernetes.io/storage-class", storageClass }
                    }
                },
                spec = new
                {
                    accessModes = new[]
                    {
                        "ReadWriteMany"
                    },
                    resources = new
                    {
                        requests = new
                        {
                            storage = size
                        }
                    }
                }
            });
        }

        internal async Task DeprovisionVolume(string name)
        {
            await client.DeletePersistentVolumeClaimAsync(name);
        }
    }
}
