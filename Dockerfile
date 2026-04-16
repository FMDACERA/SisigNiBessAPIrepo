# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SisigNiBessWebApiAdmin.csproj","./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SisigNiBessWebApiAdmin.dll"]
