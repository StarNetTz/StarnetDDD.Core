FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
COPY dist/ .
ENTRYPOINT ["dotnet", "$ext_projectname$.WebApi.dll"]
