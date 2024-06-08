#addin "nuget:?package=FluentFTP&version=28.0.5"
#addin "nuget:?package=Cake.Json&version=5.2.0"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.2"

// Not yet available via nuget
#r "./extensions/Cake.Ftp.dll"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var webArtifactsPath = "./artifacts/web/";

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

        EnsureDirectoryExists(webArtifactsPath);

        var settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            Framework = "net8.0",
            Runtime = "win-x64",
            SelfContained = true,
            OutputDirectory = webArtifactsPath,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
            {
                MaxCpuCount = 0
            }
        };

        DotNetCorePublish("../ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet.csproj", settings);

    });

Task("PopulateSecrets")
    .Does(() => {
        var secretsTargetPath = webArtifactsPath + "info.json";
        CopyFile("../ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet/App/Secrets/template.json", secretsTargetPath);

        var secrets = ParseJsonFromFile(secretsTargetPath);
        foreach (var item in secrets)
        {
            secrets[item.Key] = EnvironmentVariable(item.Key) ?? "";
        }

        SerializeJsonToFile(secretsTargetPath, secrets);
    });

Task("DeployWeb")
    .Does(async () => {

        var settings = new FtpSettings() {
            Username = EnvironmentVariable("ChesterDev_Deploy_Username"),
            Password = EnvironmentVariable("ChesterDev_Deploy_Password")
        };
        
        //Take site offline
        FtpUploadFile("chester.dev", "/httpdocs/app_offline.htm", "./artifacts/app_offline.htm", settings);
        
        // Wait 10 seconds to allow IIS to release the file locks
        await System.Threading.Tasks.Task.Delay(10000);

        var directoryToUpload = Directory(webArtifactsPath);
        FtpUploadDirectory("chester.dev", "/httpdocs/", directoryToUpload, settings);

        // Bring site back
        FtpDeleteFile("chester.dev", "/httpdocs/app_offline.htm", settings);
 
    });

Task("CreateDeployableFiles")
    .IsDependentOn("BuildAndPackageWeb")
    .IsDependentOn("PopulateSecrets");

Task("BuildAndDeployWeb")
    .IsDependentOn("CreateDeployableFiles")
    .IsDependentOn("DeployWeb");

Task("Default")
    .Does(() => {
        Warning(@"
Please specify a target. For example:    
    .\build.ps1 -Target BuildAndPackageWeb
    .\build.ps1 -Target BuildAndDeployWeb");
    });

RunTarget(target);