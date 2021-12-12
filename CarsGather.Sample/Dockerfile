FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CarsGather.Sample/CarsGather.Sample.csproj", "CarsGather.Sample/"]
RUN dotnet restore "CarsGather.Sample/CarsGather.Sample.csproj"
COPY . .
WORKDIR "/src/CarsGather.Sample"
RUN dotnet build "CarsGather.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarsGather.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarsGather.Sample.dll"]
