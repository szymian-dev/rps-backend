FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

EXPOSE 8080

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

COPY wait-for-it.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/wait-for-it.sh

ENTRYPOINT ["wait-for-it.sh", "db:5432", "--", "dotnet", "RpsApi.dll"]