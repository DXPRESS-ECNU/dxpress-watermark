# Build and publish.
$version = Read-Host 'Version'
$runtime = @("win-x64","linux-x64","osx-x64")
dotnet restore
foreach ($rt in $runtime) {
    dotnet publish -f netcoreapp2.0 -c Release -r $rt -o ./bin/Publish/$rt;
    Compress-Archive -Path .\Watermark\bin\Publish\$rt\* -DestinationPath $(".\Watermark\bin\publish\watermark_" + $version + "_" + $rt + ".zip")
}
# For pure net core
dotnet publish -f netcoreapp2.0 -c Release -o ./bin/Publish/netcore20
Compress-Archive -Path .\Watermark\bin\Publish\netcore20\* -DestinationPath $(".\Watermark\bin\publish\watermark_" + $version + "_netcore20.zip")