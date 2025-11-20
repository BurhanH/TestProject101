# Playwright Test Project with Docker

A comprehensive .NET 8.0 test automation project using Playwright and NUnit, fully containerized with Docker for consistent cross-platform test execution and detailed reporting.

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Running Tests](#running-tests)
- [Test Results](#test-results)
- [Local Development](#local-development)
- [Docker Configuration](#docker-configuration)
- [Test Configuration](#test-configuration)
- [Available Tests](#available-tests)
- [Troubleshooting](#troubleshooting)
- [CI/CD Integration](#cicd-integration)

## 🎯 Overview

This project demonstrates modern test automation practices with:

- **Playwright** - Fast, reliable end-to-end testing for web apps
- **NUnit** - Popular .NET testing framework
- **Docker** - Containerized test execution for consistency
- **Parallel Execution** - Run tests concurrently for faster feedback
- **Rich Reporting** - HTML and TRX reports with screenshots/videos on failure

**Target Website:** [playwright.dev](https://playwright.dev) - Used as a stable demo site for testing

## 🔧 Prerequisites

### Required Software

1. **Docker Desktop** (version 20.10 or later)
   - [Download for Mac](https://docs.docker.com/desktop/install/mac-install/)
   - [Download for Windows](https://docs.docker.com/desktop/install/windows-install/)
   - [Download for Linux](https://docs.docker.com/desktop/install/linux-install/)
   - Verify installation: `docker --version` and `docker compose version`

### Optional (for local development)

2. **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Verify: `dotnet --version` (should show 8.0.x)

3. **IDE** (choose one)
   - [JetBrains Rider](https://www.jetbrains.com/rider/)
   - [Visual Studio 2022](https://visualstudio.microsoft.com/)
   - [Visual Studio Code](https://code.visualstudio.com/) with C# extension

## 📁 Project Structure
```

TestProject101/
├── TestProject101-1/               # Main test project
│   ├── UnitTest1.cs                # Test cases (8 tests)
│   ├── GlobalUsings.cs             # Global using directives
│   ├── TestProject101-1.csproj     # Project file with dependencies
│   ├── Dockerfile                  # Multi-stage Docker build
│   └── .runsettings                # Test execution configuration
│
├── test-results/                   # Test output (auto-generated)
│   ├── test-results.trx            # XML test results (MSTest format)
│   ├── test-results.html           # Human-readable HTML report
│   ├── screenshots/                # Failure screenshots (if any)
│   └── videos/                     # Failure videos (if any)
│
├── compose.yaml                    # Docker Compose orchestration
├── TestProject101.sln              # Visual Studio solution file
└── README.md                       # This file
```
## 🚀 Installation

### Step 1: Clone the Repository

```bash
git clone <your-repository-url>
cd TestProject101
```

### Step 2: Verify Docker Installation

Ensure Docker Desktop is running:

```shell script
docker --version
# Expected output: Docker version 24.0.x or later

docker compose version
# Expected output: Docker Compose version v2.x.x or later
```


### Step 3: First-Time Build

Build the Docker image (this may take 5–10 minutes on the first run):

```shell script
docker compose build
```

This will:
- Download base .NET SDK image (~500MB)
- Download Playwright browser image (~1GB)
- Install .NET dependencies
- Install Chromium browser with dependencies

## ▶️ Running Tests

### Option 1: Run Tests with Docker Compose (Recommended)

Execute all tests in a clean containerized environment:

```shell script
docker compose up --build
```

**Output:** You'll see real-time test execution in the console, followed by a summary.

**Expected output:**
```
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8
```

### Option 2: Run Without Rebuilding

If you haven't changed code or dependencies:

```shell script
docker compose up
```


### Option 3: Run in Background

```shell script
docker compose up -d

# View logs
docker compose logs -f testproject101-1
```


### Option 4: Clean Environment Run

Remove all previous containers and rebuild:

```shell script
docker compose down
docker compose up --build --force-recreate
```

### Option 5: Stop Tests

```shell script
# Press Ctrl+C in the terminal, or
docker compose down
```

## 📊 Test Results

After each test run, results are saved to the `test-results/` directory.

### View HTML Report

The most user-friendly way to view results:

```shell script
# macOS
open test-results/test-results.html

# Windows
start test-results/test-results.html

# Linux
xdg-open test-results/test-results.html
```

**HTML Report includes:**
- Total tests passed/failed
- Execution time per test
- Stack traces for failures
- Links to screenshots/videos

### View TRX Report

The `test-results.trx` file is compatible with:
- Visual Studio Test Explorer
- Azure DevOps
- Jenkins
- TeamCity
- Other CI/CD platforms

### Failure Artifacts

When tests fail, Playwright captures:
- 📸 **Screenshots** - Visual state at the moment of failure
- 🎥 **Videos** - Full test recording showing what happened
- 🔍 **Traces** - Detailed timeline with network requests, console logs, etc.

Access these in the `test-results/` folder.

## 💻 Local Development

### Setup Local Environment

1. **Install .NET 8.0 SDK** (if not already installed)

2. **Restore dependencies:**
```shell script
cd TestProject101-1
dotnet restore
```

3. **Build the project:**
```shell script
dotnet build
```

4. **Install Playwright browsers:**
```shell script
# Windows
   pwsh bin/Debug/net8.0/playwright.ps1 install

   # macOS/Linux
   pwsh bin/Debug/net8.0/playwright.ps1 install
```

### Run Tests Locally

```shell script
# Run all tests
dotnet test

# Run with detailed console output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test
dotnet test --filter "FullyQualifiedName~SearchFunctionalityWorks"

# Run with custom settings
dotnet test --settings .runsettings

# Run with HTML logger (requires additional package)
dotnet test --logger "html;LogFileName=test-results.html"
```

### Debug Tests in IDE

**JetBrains Rider:**
1. Open `TestProject101.sln`
2. Navigate to `UnitTest1.cs`
3. Click the green play button next to any test method
4. Or right-click → "Debug Test"

**Visual Studio:**
1. Open `TestProject101.sln`
2. Open Test Explorer (Test → Test Explorer)
3. Right-click any test → "Debug"

**Set Breakpoints:**
- Click in the left margin of any code line
- Test will pause at breakpoint for inspection

### Running in Headed Mode (See Browser)

Edit `.runsettings` and change:
```xml
<Headless>false</Headless>
```

Then run locally (not in Docker).

## 🐳 Docker Configuration

### Dockerfile Breakdown

The `TestProject101-1/Dockerfile` uses multi-stage builds:

**Stage 1: Build**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Restores NuGet packages
# Builds the test project
```

**Stage 2: Test**
```dockerfile
FROM mcr.microsoft.com/playwright/dotnet:v1.56.0-noble AS test
# Installs .NET SDK
# Copies built artifacts
# Runs tests with multiple loggers
```

### Docker Compose Configuration

`compose.yaml` defines the service:

```yaml
services:
  testproject101-1:
    image: testproject101-1          # Image name
    build:
      context: .                     # Build from the project root
      dockerfile: TestProject101-1/Dockerfile
    volumes:
      - ./test-results:/app/TestResults  # Persist results
```


### Customizing Docker Setup

**Change Browser:** Modify `.runsettings`:
```xml
<BrowserName>firefox</BrowserName>  <!-- Options: chromium, firefox, webkit -->
```

**Add Environment Variables:** Edit `compose.yaml`:
```yaml
environment:
  - DEBUG=pw:api
  - PLAYWRIGHT_BROWSERS_PATH=/ms-playwright
```

**Resource Limits:** Add to `compose.yaml`:
```yaml
deploy:
  resources:
    limits:
      cpus: '2'
      memory: 4G
```

## ⚙️ Test Configuration

### .runsettings File

Located at `TestProject101-1/.runsettings`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <NUnit>
    <NumberOfTestWorkers>2</NumberOfTestWorkers>  <!-- Parallel test workers -->
  </NUnit>
  <RunConfiguration>
    <ResultsDirectory>/app/TestResults</ResultsDirectory>
  </RunConfiguration>
  <Playwright>
    <BrowserName>chromium</BrowserName>
    <ExpectTimeout>5000</ExpectTimeout>           <!-- Assertion timeout (ms) -->
    <LaunchOptions>
      <Headless>true</Headless>
    </LaunchOptions>
  </Playwright>
</RunSettings>
```

### Project Dependencies

Defined in `TestProject101-1.csproj`:

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.NET.Test.Sdk | 18.0.1 | Test platform |
| NUnit | 4.4.0 | Testing framework |
| NUnit3TestAdapter | 5.2.0 | VS Test Explorer integration |
| Microsoft.Playwright.NUnit | 1.56.0 | Playwright + NUnit integration |
| coverlet.collector | 6.0.4 | Code coverage |

### Global Usings

`GlobalUsings.cs` provides namespace shortcuts:

```csharp
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using Microsoft.Playwright.NUnit;
global using NUnit.Framework;
```

## 🧪 Available Tests

The project includes **8 comprehensive end-to-end tests** in `UnitTest1.cs`:

### 1. HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage
**Purpose:** Validates basic navigation flow  
**Steps:**
- Navigate to playwright.dev
- Verify the page title contains "Playwright"
- Check the "Get Started" link has the correct href
- Click the link
- Verify navigation to intro page

### 2. SearchFunctionalityWorks
**Purpose:** Tests the search feature  
**Steps:**
- Open the search dialog
- Type "api" in the search box
- Verify search results appear

### 3. NavigationMenuIsVisible
**Purpose:** Validates main navigation  
**Checks:** Docs, API, and Community links are visible

### 4. DocsPageLoadsCorrectly
**Purpose:** Verifies documentation page  
**Steps:**
- Navigate to /docs/intro
- Check the title contains "Installation"
- Verify the h1 heading is visible

### 5. CodeExampleIsDisplayed
**Purpose:** Ensures code snippets render  
**Checks:** At least one code block exists with content

### 6. LanguageTabsWork
**Purpose:** Tests language selector tabs  
**Steps:**
- Find language tab (e.g., Node.js)
- Click it
- Verify it becomes selected

### 7. FooterContainsLinks
**Purpose:** Validates footer elements  
**Checks:** GitHub link exists in footer

### 8. DarkModeToggleExists
**Purpose:** Verifies theme switcher  
**Checks:** Theme toggle button is visible

### Test Execution

**Parallel Execution:** Tests run in parallel with 2 workers (configured in `.runsettings`)

```csharp
[Parallelizable(ParallelScope.Fixtures)]
```

**Typical Execution Time:** 20-40 seconds for all 8 tests

## 🔍 Troubleshooting

### Docker Issues

**Problem:** Cannot connect to Docker daemon
```shell script
# Solution 1: Start Docker Desktop

# Solution 2: Check Docker is running
docker ps

# Solution 3: Restart Docker daemon (Linux)
sudo systemctl restart docker
```

**Problem:** Port already in use
```shell script
# Find and stop conflicting container
docker ps
docker stop <container-id>
```

**Problem:** Out of disk space
```shell script
# Clean up unused images and containers
docker system prune -a

# Check disk usage
docker system df
```

**Problem:** Build fails with network timeout
```shell script
# Use different DNS (add to Docker Desktop settings)
# Or retry build:
docker compose build --no-cache
```

### Test Failures

**Problem:** Timeout errors
```shell script
# Solution 1: Increase timeout in .runsettings
<ExpectTimeout>10000</ExpectTimeout>

# Solution 2: Check internet connection
ping playwright.dev

# Solution 3: The website may be down (check status)
```

**Problem:** Element not found
```shell script
# The website HTML structure may have changed
# Update selectors in UnitTest1.cs
```

**Problem:** All tests fail immediately
```shell script
# Rebuild with clean slate
docker compose down
docker volume prune
docker compose up --build
```

### Permission Issues

**Problem:** Cannot write to test-results/ (Linux/Mac)
```shell script
# Fix permissions
chmod -R 755 test-results/
```

**Problem:** Permission denied in Docker
```shell script
# Run container as current user
# Add to compose.yaml:
user: "${UID}:${GID}"
```

### Browser Installation Issues

**Problem:** Playwright browsers not found
```shell script
# Solution: Rebuild Docker image
docker compose build --no-cache

# Or install locally
pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps
```

## 🔄 CI/CD Integration

### GitHub Actions Example

Create `.github/workflows/test.yml`:

```yaml
name: Playwright Tests

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Run tests
        run: docker compose up --build --abort-on-container-exit
      
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: test-results/
```

### Azure DevOps Pipeline

Create `azure-pipelines.yml`:

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: Docker@2
  displayName: 'Build and Run Tests'
  inputs:
    command: 'run'
    arguments: '--rm -v $(Build.SourcesDirectory)/test-results:/app/TestResults testproject101-1'

- task: PublishTestResults@2
  condition: always()
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/test-results.trx'
```

### Jenkins Pipeline

```textmate
pipeline {
    agent any
    stages {
        stage('Test') {
            steps {
                sh 'docker compose up --build --abort-on-container-exit'
            }
        }
    }
    post {
        always {
            publishHTML([
                reportDir: 'test-results',
                reportFiles: 'test-results.html',
                reportName: 'Test Results'
            ])
        }
    }
}
```

## 📚 Additional Resources

- [Playwright .NET Documentation](https://playwright.dev/dotnet/)
- [NUnit Documentation](https://docs.nunit.org/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [Playwright Best Practices](https://playwright.dev/dotnet/docs/best-practices)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-test`)
3. Commit your changes (`git commit -m 'Add amazing test'`)
4. Push to the branch (`git push origin feature/amazing-test`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License – see the LICENSE file for details.

## 👥 Authors

Baur Urazalinov / Claude 4.5 Sonnet

## 🙏 Acknowledgments

- Microsoft Playwright Team for the excellent testing framework
- NUnit Team for the robust testing platform
- Docker Team for containerization technology

---

**Last Updated:** 2025-11-19  
**Project Version:** 1.0.0  
**Playwright Version:** 1.56.0  
**.NET Version:** 8.0
