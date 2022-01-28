<#
    Packages and deploys a plugin project as an optional package.

    Visual Studio does not support WinUI3 optional pacakges so we need to invoke some external tools to build and deploy it ourselves.
    Command line tools makeappx and signtool must be installed and must be added to the environment path.
#>
param (
    # the directoy to package, should be the output directory of the plugin class library
    [string]$targetDirectory,
    
    # the full path to the certificate used for msix package signing
    [string]$certificatePath,

    # the password to use for msix package signing
    [string]$certificatePassword
)

$packagePath = Join-Path -Path $targetDirectory -ChildPath "package.msix"

# delete the old .msix package file if it exists
Remove-Item $packagePath -ErrorAction Ignore

#delete framework dlls, these must be provided by the host application
#https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support#plugin-framework-references
Remove-Item (Join-Path -Path $targetDirectory -ChildPath "WinRT.Runtime.dll") -ErrorAction Ignore
Remove-Item (Join-Path -Path $targetDirectory -ChildPath "Microsoft.Windows.SDK.NET.dll") -ErrorAction Ignore

#rename the .pri file following convention to have it automatically loaded into the app ResourceManager resource maps
#resources.pri content can be inspected with cmd "makepri dump /dt detailed"
$defaultPriPath = Join-Path -Path $targetDirectory -ChildPath "HostApp.SimplePlugin.pri"
$conventionalPriPath = Join-Path -Path $targetDirectory -ChildPath "resources.pri"
Move-Item -Path $defaultPriPath -Destination $conventionalPriPath -Force

#create a signed package from the target directory
makeappx pack /o /d $targetDirectory /p $packagePath
signtool sign /fd SHA256 /a /f $certificatePath /p $certificatePassword $packagePath

# deploy the package locally
# TODO: get the fullname of the optional package programatically
Remove-AppxPackage -Package HostApp.SimplePlugin_1.0.0.0_neutral__hayx0sw9bh1wa
Add-AppXPackage -ForceUpdateFromAnyVersion -Path $packagePath