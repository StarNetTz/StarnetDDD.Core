FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS base
WORKDIR /app
COPY dist/ .
ENTRYPOINT ["dotnet", "$ext_projectname$.ReadModel.App.dll"]

