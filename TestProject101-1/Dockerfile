FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestProject101-1/TestProject101-1.csproj", "TestProject101-1/"]
RUN dotnet restore "TestProject101-1/TestProject101-1.csproj"
COPY . .
WORKDIR "/src/TestProject101-1"
RUN dotnet build "./TestProject101-1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM mcr.microsoft.com/playwright/dotnet:v1.56.0-noble AS test
WORKDIR /app

# Install .NET SDK
RUN apt-get update && apt-get install -y wget && \
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh && \
    chmod +x ./dotnet-install.sh && \
    ./dotnet-install.sh --channel 8.0 && \
    rm dotnet-install.sh

ENV PATH="/root/.dotnet:${PATH}"
ENV DOTNET_ROOT="/root/.dotnet"
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

# Copy the test project
COPY --from=build /app/build .
COPY TestProject101-1/.runsettings /app/.runsettings

# Create TestResults directory
RUN mkdir -p /app/TestResults

# Run tests with settings and loggers
ENTRYPOINT ["dotnet", "test", "TestProject101-1.dll", \
    "--settings:/app/.runsettings", \
    "--logger:console;verbosity=detailed", \
    "--logger:trx;LogFileName=/app/TestResults/test-results.trx", \
    "--logger:html;LogFileName=/app/TestResults/test-results.html"]