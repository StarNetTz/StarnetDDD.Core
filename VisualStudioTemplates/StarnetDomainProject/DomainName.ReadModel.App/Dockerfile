FROM microsoft/dotnet:2.1.5-runtime AS base
WORKDIR /app
COPY dist/ .
ENTRYPOINT ["dotnet", "$ext_projectname$.ReadModel.App.dll"]

