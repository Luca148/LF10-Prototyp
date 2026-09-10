# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (only the .csproj files) so this layer is cached
# as long as the dependencies don't change.
COPY LF10.Api/LF10.Api.csproj LF10.Api/
COPY HarassmentFilter.Core/HarassmentFilter.Core.csproj HarassmentFilter.Core/
RUN dotnet restore LF10.Api/LF10.Api.csproj

# Now copy the rest and publish.
COPY . .
RUN dotnet publish LF10.Api/LF10.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Keep pristine copies of the seed data so the entrypoint can
# populate / refresh the mounted Data volume.
RUN cp -r /app/Data /app/Data.seed

COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["docker-entrypoint.sh"]
CMD ["dotnet", "LF10.Api.dll"]
