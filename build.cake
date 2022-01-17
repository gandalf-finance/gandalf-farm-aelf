#tool dotnet:?package=Codecov.Tool&version=1.13.0
#addin nuget:?package=Cake.Codecov&version=1.0.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var rootPath     = "./";
var srcPath      = rootPath + "src/";
var contractPath = rootPath + "contract/";
var testPath     = rootPath + "test/";
var solution     = rootPath + "Awaken.Contracts.Farm.sln";

Task("Clean")
    .Description("clean up project cache")
    .Does(() =>
{
    CleanDirectories(srcPath + "**/bin");
    CleanDirectories(srcPath + "**/obj");
    CleanDirectories(contractPath + "**/bin");
    CleanDirectories(contractPath + "**/obj");
    CleanDirectories(testPath + "**/bin");
    CleanDirectories(testPath + "**/obj");
});

Task("Restore")
    .Description("restore project dependencies")
    .Does(() =>
{
    DotNetCoreRestore(solution, new DotNetCoreRestoreSettings
    {
        Verbosity = DotNetCoreVerbosity.Quiet,
        Sources = new [] { "https://www.myget.org/F/aelf-project-dev/api/v3/index.json", "https://api.nuget.org/v3/index.json" }
    });
});
Task("Build")
    .Description("Compilation project")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var buildSetting = new DotNetCoreBuildSettings{
        NoRestore = true,
        Configuration = configuration,
        ArgumentCustomization = args => {
            return args.Append("/clp:ErrorsOnly")
                       .Append("-v quiet");}
    };
     
    DotNetCoreBuild(solution, buildSetting);
});

Task("Test-with-Codecov")
    .Description("operation tes")
    .IsDependentOn("Build")
    .Does(() =>
{
    var testSetting = new DotNetCoreTestSettings{
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        ArgumentCustomization = args => {
                    return args
                        .Append("--logger trx")
                        .Append("--settings CodeCoverage.runsettings")
                        .Append("--collect:\"XPlat Code Coverage\"");
                }                  
    };
    var testProjects = GetFiles("./test/*.Tests/*.csproj");
    var testProjectList = testProjects.OrderBy(p=>p.FullPath).ToList();
    foreach(var testProject in testProjectList)
    {
        DotNetCoreTest(testProject.FullPath, testSetting);
    }
});

Task("Upload-Coverage-Azure")
    .Does(() =>
{
    Codecov("./CodeCoverage/Cobertura.xml",EnvironmentVariable("CODECOV_TOKEN"));
});

RunTarget(target);