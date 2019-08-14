rem nuget pack "src\IRO.XWebView.Droid\IRO.XWebView.Droid.csproj"
rem nuget pack -IncludeReferencedProjects "package.nuspec" -Properties version="1.0.1"
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" -t:pack "src\IRO.XWebView.Droid\IRO.XWebView.Droid.csproj" /t:pack /p:Configuration=Release
pause