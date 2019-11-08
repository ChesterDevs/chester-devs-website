#addin "nuget:?package=Cake.WebDeploy&version=0.3.4"
#addin "nuget:?package=SharpZipLib&version=1.2.0"
#addin "nuget:?package=Cake.Compression&version=0.2.4"


var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var webArtifactsPath = "./work/web/";
var webArtifactsOutputFolder = "./artifacts/";
var webArtifactsOutputFile = "./artifacts/web.zip";

Task("CleanWeb")
    .Does(() => {
        DotNetCoreClean("../ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet/");
    });

Task("CleanDeploymentPackageWeb")
    .Does(() => {
        CleanDirectory(webArtifactsPath);
    });
    
Task("BuildAndPackageWeb")
    .IsDependentOn("CleanWeb")
    .IsDependentOn("CleanDeploymentPackageWeb")
    .Does(() => {

        var settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            Framework = "netcoreapp2.1",
            Runtime = "win-x64",
            SelfContained = true,
            OutputDirectory = webArtifactsPath,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
            {
                MaxCpuCount = 0
            }
        };

        DotNetCorePublish("../ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet.csproj", settings);

        EnsureDirectoryExists(webArtifactsOutputFolder);
        ZipCompress(webArtifactsPath, webArtifactsOutputFile);
    });

Task("DeployWeb")
    .Does(() => {
 
        DeployWebsite(new DeploySettings()
        {
            SourcePath = webArtifactsOutputFile,
            Delete = false,
            UseAppOffline = false,
            UseChecksum = true,
            AllowUntrusted = true,
            NTLM = true,
            ComputerName = "chester.dev",
            SiteName = "chester.dev",
            Username = EnvironmentVariable("ChesterDev_Deploy_Username"),
            Password = EnvironmentVariable("ChesterDev_Deploy_Password")
        });

    });

Task("BuildAndDeployWeb")
    .IsDependentOn("BuildAndPackageWeb")
    .IsDependentOn("DeployWeb");

Task("Default")
    .Does(() => {
        Warning(@"
Please specify a target. For example:    
    .\build.ps1 -Target BuildAndPackageWeb
    .\build.ps1 -Target BuildAndDeployWeb");
    });

RunTarget(target);