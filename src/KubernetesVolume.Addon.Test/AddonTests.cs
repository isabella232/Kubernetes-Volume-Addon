namespace KubernetesVolume.Addon.Test
{
    using System.Collections.Generic;
    using Apprenda.SaaSGrid.Addons;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AddonTests
    {
        [TestMethod]
        public void AddonTest()
        {
            // Arrange
            var request = new AddonTestRequest
            {
                DeveloperParameters = new[]
                {
                    new AddonParameter
                    {
                        Key = "VolumeSize",
                        Value = "1Gi"
                    }
                },
                Manifest = new AddonManifest
                {
                    ProvisioningLocation = "https://ash:6443",
                    Properties = new List<AddonProperty>
                    {
                        new AddonProperty
                        {
                            Key = AddonManifest.InstanceAliasPropertyKey,
                            Value = "test"
                        },
                        new AddonProperty
                        {
                            Key = "StorageClass",
                            Value = "glusterfs"
                        },
                        new AddonProperty
                        {
                            Key = "Namespace",
                            Value = "acp"
                        },
                        new AddonProperty
                        {
                            Key = "Certificate",
                            Value = "client.pfx"
                        },
                        new AddonProperty
                        {
                            Key = "Password",
                            Value = "password"
                        }
                    }
                }
            };
            var addon = new Addon();

            // Act
            var result = addon.Test(request);

            // Assert
            Assert.IsTrue(result.IsSuccess, result.EndUserMessage);
        }
    }
}
