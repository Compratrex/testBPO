FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY WholesalePurchasingPlatform.sln ./
COPY src/WholesalePlatform.Domain/WholesalePlatform.Domain.csproj src/WholesalePlatform.Domain/
COPY src/WholesalePlatform.Application/WholesalePlatform.Application.csproj src/WholesalePlatform.Application/
COPY src/WholesalePlatform.Infrastructure/WholesalePlatform.Infrastructure.csproj src/WholesalePlatform.Infrastructure/
COPY src/WholesalePlatform.WebApi/WholesalePlatform.WebApi.csproj src/WholesalePlatform.WebApi/
COPY tests/WholesalePlatform.Tests/WholesalePlatform.Tests.csproj tests/WholesalePlatform.Tests/

RUN dotnet restore WholesalePurchasingPlatform.sln

COPY . .
RUN dotnet publish src/WholesalePlatform.WebApi/WholesalePlatform.WebApi.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WholesalePlatform.WebApi.dll"]
