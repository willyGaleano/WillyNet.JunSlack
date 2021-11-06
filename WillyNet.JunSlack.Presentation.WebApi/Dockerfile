#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WillyNet.JunSlack.Presentation.WebApi/WillyNet.JunSlack.Presentation.WebApi.csproj", "WillyNet.JunSlack.Presentation.WebApi/"]
RUN dotnet restore "WillyNet.JunSlack.Presentation.WebApi/WillyNet.JunSlack.Presentation.WebApi.csproj"
COPY . .
WORKDIR "/src/WillyNet.JunSlack.Presentation.WebApi"
RUN dotnet build "WillyNet.JunSlack.Presentation.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WillyNet.JunSlack.Presentation.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WillyNet.JunSlack.Presentation.WebApi.dll"]