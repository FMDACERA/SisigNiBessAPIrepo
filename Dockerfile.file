# Use the official .NET image as a build stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SisigNiBessWebApiAdmin/SisigNiBessWebApiAdmin.csproj", "SisigNiBessWebApiAdmin/"]
RUN dotnet restore "SisigNiBessWebApiAdmin/SisigNiBessWebApiAdmin.csproj"
COPY . .
WORKDIR "/src/SisigNiBessWebApiAdmin"
RUN dotnet build "SisigNiBessWebApiAdmin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SisigNiBessWebApiAdmin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SisigNiBessWebApiAdmin.dll"]
