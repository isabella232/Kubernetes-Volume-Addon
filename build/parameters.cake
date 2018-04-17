public class BuildParameters
{
    public string Target { get; set; }
    public string Configuration { get; set; }

    public string OutputPath { get; set; }

    public string Solution { get; set; }
    public string AddonProjectFile { get; set; }

    public string AddonArchivePath { get; set; }
    public string AddonBinariesPath { get; set; }
    public string AddonManifest { get; set; }
    public string AddonArchive { get; set; }

    public static BuildParameters Load(ICakeContext context)
    {
        var target = context.Argument("Target", "Prepare-Artifacts");
        var configuration = context.Argument("Configuration", "Release");
        var outputPath = "./output";
        var stagingPath = "./staging";
        var sourcePath = "./src";
        var manifestsPath = "./manifests";
        var addonName = "KubernetesVolume.Addon";

        return new BuildParameters
        {
            Target = target,
            Configuration = configuration,
            OutputPath = outputPath,
            Solution = "./KubernetesVolumeAddon.sln",
            AddonProjectFile = $"{sourcePath}/{addonName}/{addonName}.csproj",
            AddonArchivePath = $"{stagingPath}/addon",
            AddonBinariesPath = $"{sourcePath}/{addonName}/bin/{configuration}",
            AddonManifest = $"{manifestsPath}/AddonManifest.xml",
            AddonArchive = $"{outputPath}/addon.zip",
        };
    }
}