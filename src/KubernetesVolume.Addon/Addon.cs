namespace KubernetesVolume.Addon
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apprenda.SaaSGrid.Addons;

    public class Addon : AddonBase
    {
        public override ProvisionAddOnResult Provision(AddonProvisionRequest request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var volumeSize = request.DeveloperParameters.First(p => p.Key == "VolumeSize").Value;

                    await new Logic(request.Manifest).ProvisionVolume(
                        request.Manifest.InstanceAlias, volumeSize);

                    return ProvisionAddOnResult.Success(request.Manifest.InstanceAlias);
                }
                catch (Exception ex)
                {
                    return ProvisionAddOnResult.Failure($"Error provisioning Kubernetes volume: {ex}");
                }
            }).GetAwaiter().GetResult();
        }

        public override OperationResult Deprovision(AddonDeprovisionRequest request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await new Logic(request.Manifest).DeprovisionVolume(request.Manifest.InstanceAlias);

                    return new OperationResult { IsSuccess = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult { EndUserMessage = $"Error deprovisioning Kubernetes volume: {ex}" };
                }
            }).GetAwaiter().GetResult();
        }

        public override OperationResult Test(AddonTestRequest request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var logic = new Logic(request.Manifest);
                    var volumeSize = request.DeveloperParameters.First(p => p.Key == "VolumeSize").Value;
                    var name = Guid.NewGuid().ToString();

                    // Provision
                    await logic.ProvisionVolume(name, volumeSize);

                    // Deprovision
                    await logic.DeprovisionVolume(name);

                    return new OperationResult { IsSuccess = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult { EndUserMessage = $"Error testing the add-on: {ex}" };
                }
            }).GetAwaiter().GetResult();
        }
    }
}
