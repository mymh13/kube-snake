FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["guestbook.csproj", "./"]
RUN dotnet restore "guestbook.csproj"
COPY . .
RUN dotnet build "guestbook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "guestbook.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "guestbook.dll"]