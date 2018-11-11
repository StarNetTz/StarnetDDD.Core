FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS base
WORKDIR /app
COPY dist/ .
ENTRYPOINT ["dotnet", "$ext_projectname$.WebApi.dll"]
