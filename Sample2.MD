# Sample 2

## Code

```csharp
using System;
using System.Runtime.InteropServices;
using System.Text;

public class WindowsVaultService
{
    // P/Invoke for writing to the Windows Credential Manager
    [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredWrite([In] ref NativeCredential userCredential, [In] uint flags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeCredential
    {
        public uint Flags;
        public uint Type; // 2 = Domain Password (NTLM)
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist; // 2 = Local Machine Persistence
        public uint AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    public static void AddNtlmCredential(string domain, string username, string password)
    {
        var cred = new NativeCredential { Type = 2, Persist = 2 };
        
        // TargetName is the URL or Server Name the browser checks
        cred.TargetName = Marshal.StringToCoTaskMemUni(domain);
        cred.UserName = Marshal.StringToCoTaskMemUni(username);

        byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
        cred.CredentialBlobSize = (uint)passwordBytes.Length;
        cred.CredentialBlob = Marshal.AllocCoTaskMem(passwordBytes.Length);
        Marshal.Copy(passwordBytes, 0, cred.CredentialBlob, passwordBytes.Length);

        if (!CredWrite(ref cred, 0))
        {
            int error = Marshal.GetLastWin32Error();
            throw new Exception($"Vault Injection Failed. Win32 Error: {error}");
        }
        Console.WriteLine($"[Success] Credentials injected for {domain}");
    }
}
```

## Docker

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

## Docker Compose

```docker
version: '3.8'
services:
  ntlm-bot:
    build: .
    image: my-windows-playwright:latest
    environment:
      - NTLM_DOMAIN=your-internal-site.com
      - NTLM_USER=DOMAIN\user
      - NTLM_PASS=password123
    # Windows containers require high shm-size for Chrome GUI/Headless
    shm_size: '2gb'
    deploy:
      resources:
        limits:
          memory: 4G
```

## Code Test

```csharp
public async Task RunAutomation()
{
    // 1. Get credentials from Docker environment
    var domain = Environment.GetEnvironmentVariable("NTLM_DOMAIN");
    var user = Environment.GetEnvironmentVariable("NTLM_USER");
    var pass = Environment.GetEnvironmentVariable("NTLM_PASS");

    // 2. Inject into OS Vault
    WindowsVaultService.AddNtlmCredential(domain, user, pass);

    // 3. Launch Playwright
    using var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    
    // No context-level HttpCredentials needed; browser uses the Vault!
    var page = await browser.NewPageAsync();
    await page.GotoAsync($"https://{domain}");
    
    Console.WriteLine("Navigated successfully without NTLM popup intervention.");
}
```
