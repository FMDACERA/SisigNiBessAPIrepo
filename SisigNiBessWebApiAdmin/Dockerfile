FROM ://microsoft.com AS build-env
WORKDIR /app

# This finds any .csproj file in the current folder or subfolders
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM ://microsoft.com
WORKDIR /app
COPY --from=build-env /app/out .
# Ensure this matches your project name exactly
ENTRYPOINT ["dotnet", "SisigNiBessWebApiAdmin.dll"]