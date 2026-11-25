# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (only the .csproj files) so this layer is cached
# as long as the dependencies don't change.
COPY LF10.Api/LF10.Api.csproj LF10.Api/
COPY HarassmentFilter.Core/HarassmentFilter.Core.csproj HarassmentFilter.Core/
COPY LF10.Data/LF10.Data.csproj LF10.Data/
RUN dotnet restore LF10.Api/LF10.Api.csproj

# Now copy the rest and publish.
COPY . .
RUN dotnet publish LF10.Api/LF10.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "LF10.Api.dll"]
