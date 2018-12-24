# Just a zip tool now.
$version = Read-Host 'Version'
$pubfileList = Get-ChildItem  '.\Watermark\Properties\PublishProfiles\' -recurse -Include *.pubxml
msbuild -t:restore
foreach ($file in $pubfileList) {
    $pubName = [System.IO.Path]::GetFileNameWithoutExtension($file);
    # msbuild .\Watermark\Watermark.csproj /p:DeployOnBuild=true /p:PublishProfile=$pubName;
	# Cannot run on msbuild 15;
    Compress-Archive -Path .\Watermark\bin\publish\$pubName\* -DestinationPath $(".\Watermark\bin\publish\watermark_" + $version + "_" + $pubName + ".zip")
}