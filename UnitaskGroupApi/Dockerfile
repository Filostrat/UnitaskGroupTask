FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["UnitaskGroupApi/UnitaskGroupApi.csproj", "UnitaskGroupApi/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "UnitaskGroupApi/UnitaskGroupApi.csproj"

COPY . .

WORKDIR /src/UnitaskGroupApi
RUN dotnet publish "UnitaskGroupApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UnitaskGroupApi.dll"]
