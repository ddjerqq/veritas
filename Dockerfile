FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt update
RUN apt install -y curl


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# copy
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Presentation/Presentation.csproj", "Presentation/"]

# restore
RUN dotnet restore "Domain/Domain.csproj"
RUN dotnet restore "Application/Application.csproj"
RUN dotnet restore "Infrastructure/Infrastructure.csproj"
RUN dotnet restore "Presentation/Presentation.csproj"

COPY /src .

# build
RUN dotnet build -c ${BUILD_CONFIGURATION} -o /app/build --no-restore "Domain/Domain.csproj"
RUN dotnet build -c ${BUILD_CONFIGURATION} -o /app/build --no-restore "Application/Application.csproj"
RUN dotnet build -c ${BUILD_CONFIGURATION} -o /app/build --no-restore "Infrastructure/Infrastructure.csproj"
RUN dotnet build -c ${BUILD_CONFIGURATION} -o /app/build --no-restore "Presentation/Presentation.csproj"

# publish
RUN dotnet publish -c ${BUILD_CONFIGURATION} -o /app/publish --no-restore "Domain/Domain.csproj"
RUN dotnet publish -c ${BUILD_CONFIGURATION} -o /app/publish --no-restore "Application/Application.csproj"
RUN dotnet publish -c ${BUILD_CONFIGURATION} -o /app/publish --no-restore "Infrastructure/Infrastructure.csproj"
RUN dotnet publish -c ${BUILD_CONFIGURATION} -o /app/publish --no-restore "Presentation/Presentation.csproj"


FROM base AS final
EXPOSE 8000

HEALTHCHECK --interval=30s --timeout=10s --retries=3 CMD curl --silent --fail http://localhost:8000/api/v1/health || exit 1

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]
