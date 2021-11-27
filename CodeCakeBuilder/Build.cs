using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Core;

using Cake.Core.Diagnostics;
using Cake.Core.IO;
using SimpleGitVersion;
using System.Linq;



namespace CodeCake
{
    
    public partial class Build : CodeCakeHost
    {
        public Build()
        {
            Cake.Log.Verbosity = Verbosity.Diagnostic;

            StandardGlobalInfo globalInfo = CreateStandardGlobalInfo()
                                                .AddDotnet()
                                                .SetCIBuildTag();

            Task( "Check-Repository" )
                .Does( () =>
                {
                    globalInfo.TerminateIfShouldStop();
                } );
                
            Task( "Clean" )
                .IsDependentOn( "Check-Repository" )
                .Does( () =>
                {
                    globalInfo.GetDotnetSolution().Clean();
                    Cake.CleanDirectories( globalInfo.ReleasesFolder.ToString() );
                   
                } );

            Task( "Build" )
                .IsDependentOn( "Check-Repository" )
                .IsDependentOn( "Clean" )
                .Does( () =>
                {
                    globalInfo.GetDotnetSolution().Build();
                } );

            Task( "Create-NuGet-Packages" )
                .WithCriteria( () => globalInfo.IsValid )
                .IsDependentOn( "Build" )
                .Does( () =>
                {
                    globalInfo.GetDotnetSolution().Pack();
                } );

            Task( "Push-Artifacts" )
                .IsDependentOn( "Create-NuGet-Packages" )
                .WithCriteria( () => globalInfo.IsValid )
                .Does( async () =>
                {
                    await globalInfo.PushArtifactsAsync();
                } );

            // The Default task for this script can be set here.
            Task( "Default" )
                .IsDependentOn( "Push-Artifacts" );
        }

    }
}
