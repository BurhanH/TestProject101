namespace TestProject101_1;

[Parallelizable(ParallelScope.Fixtures)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        // create a locator
        var getStarted = Page.Locator("text=Get Started");

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }

    [Test]
    public async Task SearchFunctionalityWorks()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Click on the search button
        var searchButton = Page.Locator("button[aria-label='Search']").First;
        await searchButton.ClickAsync();

        // Type in search box
        var searchInput = Page.Locator("input[placeholder='Search docs']");
        await searchInput.FillAsync("api");

        // Verify search results appear
        var searchResults = Page.Locator(".DocSearch-Dropdown");
        await Expect(searchResults).ToBeVisibleAsync();
    }

    [Test]
    public async Task NavigationMenuIsVisible()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Check main navigation items are visible
        var docsLink = Page.Locator("nav >> text=Docs");
        await Expect(docsLink).ToBeVisibleAsync();

        var apiLink = Page.Locator("nav >> text=API");
        await Expect(apiLink).ToBeVisibleAsync();

        var communityLink = Page.Locator("nav >> text=Community");
        await Expect(communityLink).ToBeVisibleAsync();
    }

    [Test]
    public async Task DocsPageLoadsCorrectly()
    {
        await Page.GotoAsync("https://playwright.dev/docs/intro");

        // Verify page title
        await Expect(Page).ToHaveTitleAsync(new Regex("Installation"));

        // Verify main heading is visible
        var heading = Page.Locator("h1");
        await Expect(heading).ToBeVisibleAsync();
        await Expect(heading).ToContainTextAsync("Installation");
    }

    [Test]
    public async Task CodeExampleIsDisplayed()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Scroll to code examples section
        var codeBlock = Page.Locator("pre code").First;
        await Expect(codeBlock).ToBeVisibleAsync();

        // Verify it contains code
        var codeText = await codeBlock.TextContentAsync();
        Assert.That(codeText, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task LanguageTabsWork()
    {
        await Page.GotoAsync("https://playwright.dev/docs/intro");

        // Find and click on different language tabs
        var nodejsTab = Page.Locator("button:has-text('Node.js')").First;
        if (await nodejsTab.IsVisibleAsync())
        {
            await nodejsTab.ClickAsync();
            await Expect(nodejsTab).ToHaveAttributeAsync("aria-selected", "true");
        }
    }

    [Test]
    public async Task FooterContainsLinks()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Scroll to footer
        await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");

        // Check for GitHub link in footer
        var githubLink = Page.Locator("footer a[href*='github.com']").First;
        await Expect(githubLink).ToBeVisibleAsync();
    }

    [Test]
    public async Task DarkModeToggleExists()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Look for theme toggle button
        var themeToggle = Page.Locator("button[title*='theme' i], button[aria-label*='theme' i]").First;
        await Expect(themeToggle).ToBeVisibleAsync();
    }
}