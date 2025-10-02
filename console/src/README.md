# TestRepo3

Generated with ATDD Accelerator

- System Language: java
- System Test Language: dotnet

# Instructions

dotnet clean
dotnet build 
.\src\bin\Debug\net8.0\Optivem.AtddAccelerator.TemplateGenerator.exe generate monorepo --repository-name Repo$([guid]::NewGuid().ToString('N').Substring(0,8)) --system-language dotnet --system-test-language typescript --github-username valentinajemuovic
