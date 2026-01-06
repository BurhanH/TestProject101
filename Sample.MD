# Sample

## Code

```Csharp
using Microsoft.Playwright;
using System.Threading.Tasks;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        
        // Launch Chrome. In Docker, 'Headless = true' is usually 
        // preferred unless you have a specific reason for headed.
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
            Headless = true 
        });

        // The "Secret Sauce": Define credentials at the Context level
        // This handles NTLM, Kerberos, and Basic auth automatically.
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            HttpCredentials = new HttpCredentials
            {
                Username = @"YOURDOMAIN\YourUsername",
                Password = "YourPassword123"
            },
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        var page = await context.NewPageAsync();

        // Navigate to the site. 
        // There is no popup to handle; Playwright sends the headers directly.
        await page.GotoAsync("https://your-internal-ntlm-site.com");

        // Verify login success by looking for a web element
        var welcomeMessage = page.Locator("#welcome-header");
        await welcomeMessage.WaitForAsync();
        
        System.Console.WriteLine("Successfully logged in using NTLM!");
        
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });
    }
}
```

## Dockerfile

```docker
# Use a lighter base if possible
FROM mcr.microsoft.com/dotnet/sdk:6.0-windowsservercore-ltsc2022

# Install Chrome
RUN powershell -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1')); choco install googlechrome -y"

WORKDIR /app
COPY . .
RUN dotnet build -c Release

# Run the app
ENTRYPOINT ["dotnet", "run", "--project", "YourProject.csproj"]
```

## Docker-Compose

```docker
version: '3.8'
services:
  playwright-bot:
    image: your-windows-playwright-image
    build: .
    environment:
      # These will be passed to your C# app at runtime
      - NTLM_USER=DOMAIN\YourServiceAccount
      - NTLM_PASS=YourSecretPassword123
    # Windows containers often need extra memory for Chrome
    deploy:
      resources:
        limits:
          memory: 4G
```

## .env

```txt
NTLM_USER=DOMAIN\user
NTLM_PASS=password123
```
