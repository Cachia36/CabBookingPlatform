ARG SERVICE

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG SERVICE

WORKDIR /src
COPY . .

RUN dotnet restore "${SERVICE}/${SERVICE}.csproj"

RUN dotnet publish "${SERVICE}/${SERVICE}.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG SERVICE

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV APP_DLL=${SERVICE}.dll

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet $APP_DLL"]