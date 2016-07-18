//Addins
#addin Cake.VersionReader
#addin Cake.FileHelpers
#addin Cake.Git

//Variables
var target = Argument ("target", "Default");
var buildType = Argument("Configuration", "Release");

var corePath = "./Core/Core.csproj";
var coreBin = "./bin/";
var coreSpec = "./Core/Core.nuspec";
var coreBinary = "./Core/bin/Release/LiveCharts.dll";

var buildTags = new string[] { "Debug", "403", "45", "451", "452", "46", "461" };
var buildDirectories = new string[] { "./bin/Debug", "./bin/net403", "./bin/net45", "./bin/net451", "./bin/net452", "./bin/net46", "./bin/net461" };
var configurationList = new string[] { "Debug", "net40", "net45", "net451", "net452", "net46", "net461" };

var wpfBinDirectory = "./WpfView/bin";
var wpfPath = "./WpfView/wpfview.csproj";
var wpfSpec = "./WpfView/WpfView.nuspec";
var wpfBinary = "./WpfView/bin/net403/LiveCharts.Wpf.dll";

var formsBinDirectory = "./WinFormsView/bin";
var formsPath = "./WinFormsView/WinFormsView.csproj";
var formsSpec = "./WinFormsView/WinFormsView.nuspec";
var formsBinary = "./WinFormsView/bin/net403/LiveCharts.WinForms.dll";

//Main Tasks

//Print out configuration
Task("OutputArguments")
    .Does(() =>
    {
        Information("Target: " + target);
        Information("Build Type: " + buildType);
    });

//Build Core
Task("Core")
    .Does(() =>
    {
        Information("-- Core - " + buildType.ToUpper() + " --");
        var ouputDirectory = coreBin + buildType;
        if(!DirectoryExists(ouputDirectory))
        {
            CreateDirectory(ouputDirectory);
        }

        BuildProject(corePath, ouputDirectory, buildType);
        
        if(buildType == "Release")
        {
            NugetPack(coreSpec, coreBinary);
        }
        Information("-- Core Packed --");
    });

//Build WPF
Task("WPF")
    .Does(() =>
    {
        if(!DirectoryExists(wpfBinDirectory))
        {
            CreateDirectory(wpfBinDirectory);
        }

        for(var r = 0; r < buildTags.Length; r++)
        {
            Information("-- WPF " + buildTags[r].ToUpper() + " --");
            if(!DirectoryExists(buildDirectories[r]))
            {
                CreateDirectory(buildDirectories[r]);
            }
            BuildProject(wpfPath, buildDirectories[r], configurationList[r]);
        }

        if(buildType == "Release")
        {
            NugetPack(wpfSpec, wpfBinary);
        }
        Information("-- WPF Packed --");
    });

Task("WinForms")
    .Does(() => 
    {
        if(!DirectoryExists(formsBinDirectory))
        {
            CreateDirectory(formsBinDirectory);
        }

        for(var r = 0; r < buildTags.Length; r++)
        {
            Information("-- WinForms " + buildTags[r].ToUpper() + " --");
            if(!DirectoryExists(buildDirectories[r]))
            {
                CreateDirectory(buildDirectories[r]);
            }
            BuildProject(formsPath, buildDirectories[r], configurationList[r]);
        }

        if(buildType == "Release")
        {
            NugetPack(formsSpec, formsBinary);
        }
        Information("-- WinForms Packed --");
    });

Task("Default")
    .IsDependentOn("OutputArguments")
	.IsDependentOn("Core")
    .IsDependentOn("WPF")
    .IsDependentOn("WinForms");

//Entry point for Cake build
RunTarget (target);

//Helper Methods

//Build a project
public void BuildProject(string path, string outputPath, string configuration)
{
    Information("Building " + path);
    try
    {
        DotNetBuild(path, settings =>
        settings.SetConfiguration(configuration)
        .WithProperty("Platform", "AnyCPU")
        .WithTarget("Clean,Build")
        .WithProperty("OutputPath", outputPath)
        .SetVerbosity(Cake.Core.Diagnostics.Verbosity.Minimal)
        );
    }
    catch(Exception ex)
    {
        Error("An error occured while trying to build {0} with {1}", path, configuration);
    }

    Information("Build completed");
}

//Pack into Nuget package
public void NugetPack(string nuspecPath, string mainBinaryPath)
{
    Information("Packing " + nuspecPath);
    var binary = MakeAbsolute(File(mainBinaryPath));
    var binaryVersion = GetFullVersionNumber(binary);
    ReplaceRegexInFiles(nuspecPath, "0.0.0.0", binaryVersion);
    
    NuGetPack(nuspecPath, new NuGetPackSettings{
        Verbosity = NuGetVerbosity.Quiet,
        OutputDirectory = "./"
    });

    //We revert the nuspec file to the check out one, otherwise we cannot build it again with a new version
    //This should rather use XmlPoke but cannot yet get it to work
    var fullNuspecPath = MakeAbsolute(File(nuspecPath));
    GitCheckout("./", fullNuspecPath);

    Information("Packing completed");
}