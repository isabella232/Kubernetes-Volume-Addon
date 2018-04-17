#tool "nuget:?package=Cake.FileHelpers"

#load "./build/parameters.cake"

var Parameters = BuildParameters.Load(Context);

Setup(context => {
    Information($"Running build: {Parameters.Target} {Parameters.Configuration}");
});

Task("Restore-Nuget-Packages")
    .Does(() => {
        NuGetRestore(Parameters.Solution);
    });

Task("Build-Addon")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() => {
        MSBuild(Parameters.AddonProjectFile, new MSBuildSettings {
            Configuration = Parameters.Configuration
        });
    });

Task("Build")
    .IsDependentOn("Build-Addon");

Task("Prep-Output-Directory")
    .Does(() => {
        EnsureDirectoryExists(Parameters.OutputPath);
        CleanDirectory(Parameters.OutputPath);
    });

Task("Create-Addon-Archive")
    .IsDependentOn("Prep-Output-Directory")
    .Does(() => {
        var addonManifest = $"{Parameters.AddonArchivePath}/AddonManifest.xml";

        EnsureDirectoryExists(Parameters.AddonArchivePath);
        CleanDirectory(Parameters.AddonArchivePath);

        CopyDirectory(Parameters.AddonBinariesPath, Parameters.AddonArchivePath);
        DeleteFiles($"{Parameters.AddonArchivePath}/*.pdb");
        CopyFile(Parameters.AddonManifest, addonManifest);
        Zip(Parameters.AddonArchivePath, Parameters.AddonArchive);
    });

Task("Copy-Docs")
    .IsDependentOn("Prep-Output-Directory")
    .Does(() => {
        CopyFiles("./LICENSE", Parameters.OutputPath);
        CopyFiles("./README.md", Parameters.OutputPath);
        CopyFiles("./CHANGELOG.md", Parameters.OutputPath);
    });

Task("Package")
    .IsDependentOn("Create-Addon-Archive")
    .IsDependentOn("Copy-Docs");

Task("Prepare-Artifacts")
    .IsDependentOn("Build")
    .IsDependentOn("Package");

RunTarget(Parameters.Target);