#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Kunlun.LPS.Worker.Console/Kunlun.LPS.Worker.Console.csproj", "src/Kunlun.LPS.Worker.Console/"]
RUN dotnet restore "src/Kunlun.LPS.Worker.Console/Kunlun.LPS.Worker.Console.csproj"
COPY . .
WORKDIR "/src/src/Kunlun.LPS.Worker.Console"
RUN dotnet build "Kunlun.LPS.Worker.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kunlun.LPS.Worker.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kunlun.LPS.Worker.Console.dll"]