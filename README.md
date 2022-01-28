# WinUI3 Plugin Sample

This sample app demonstrates a plugin solution for WinUI3 applications. The host application is a WinUI3 desktop application project. A plugin is a WinUI3 class project that uses a custom build script to pack and install the content as an optional package targeting the host application.

The plugin assembly code is loaded via reflection and an `AssemblyLoadContext` which isolates plugin dependencies from the host application. This is based on https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support.

The plugin UI Xaml is packaged in the plugin optional package. The default `InitializeComponent()` method in `UserControl` code behind files incorrectly searches the host application package for Xaml data. Each plugin `UserControl` must replace this with a workaround which instead locates the data using the full path to the Xaml file in the plugin package. See `SimpleUI.xaml.cs` for an example.

## Known limitations

* Plugins cannot introduce their own framework. Framework assemblies must be provided by the host application. https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support#plugin-framework-references
* Plugins cannot use the resource system e.g. strings/localization/images*
* Plugins cannot use 3rd party controls*

*URI location of data in the plugin package's `resources.pri` does not work. The plugin's `resources.pri` file is loaded into the host application's package graph at runtime (inspect `ResourceManager.Current` to confirm for yourself) however the data is never located correctly by URI. This means any feature dependent on the plugin's `resources.pri` will not work correctly. This includes resources defined in a `.resw` file and 3rd party controls which embed their Xaml in the `resources.pri` file.

Please contact me if you have a solution for the above issue! :)

## Build instructions

1. Before the first build you will need to create and install a certificate for msix package signing. See instructions in section below. Then modify HostApp.SimplePlugin.csproj to use your certificate path and password.
2. Build and Deploy HostApp.csproj
3. Build HostApp.SimplePlugin.csproj (the custom post build script will deploy it as an optional package to your local machine)

## Create and export an MSIX signing certificate

https://docs.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing

In PowerShell run the following to generate a new certificate. -Subject must match the Publisher property of AppxManifest.xml in the plugin project.

```powershell
New-SelfSignedCertificate -Type Custom -Subject "CN=WilliamF" -KeyUsage DigitalSignature -FriendlyName "my certificate" -CertStoreLocation "Cert:\CurrentUser\My" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
```

In PowerShell run the following to export the new certificate.

1. ```powershell
   Set-Location Cert:\CurrentUser\My
   ```

2. ```powershell
   Get-ChildItem | Format-Table Subject, FriendlyName, Thumbprint
   ```

3. Note the Thumbprint value of your certificate e.g. `BDDC57EBFEECECC2B17448FB4AB94F1F307DF5F4` and replace <Certificate Thumbprint> in below commands.

4. Choose a password and replace <My Password> with your chosen password in below commands.

5. Choose a path to export your certificate and replace <FilePath> with your chosen path in the below commands.

6. ```powershell
   $password = ConvertTo-SecureString -String <My Password> -Force -AsPlainText
   ```

7. ```powershell
   Export-PfxCertificate -cert "Cert:\CurrentUser\My\<Certificate Thumbprint>" -FilePath <FilePath>.pfx -Password $password
   ```

Modify the `HostApp.SimplePlugin.csproj` PostBuild script arguments to include the path to the exported certificate and the password you chose.

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
	<Exec Command="powershell -file $(MSBuildProjectDirectory)\build\deploy.ps1 $(TargetDir) my_certificate_path my_password" />
</Target>
```

