using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace PlaywrightSpecflowNUnit.POM.Pages
{
    public class HomePage
    {
        private IPage _page;
        private ScenarioContext _scenarioContext;
        public HomePage(IPage page, ScenarioContext scenarioContext) {
            _page = page;
            _scenarioContext = scenarioContext;
        }

        #region Locators

        #endregion

        #region Methods
        public async Task NavigateToApp()
        {
            await _page.GotoAsync("https://www.bbc.com/weather");
        }

        public async Task SearchWeatherForCity(string city)
        {
            await _page.GetByPlaceholder("Enter a city").ClickAsync();
            await _page.GetByPlaceholder("Enter a city").FillAsync(city);
            await _page.GetByPlaceholder("Enter a city").PressAsync("Enter");
            await _page.GetByRole(AriaRole.Link, new() { Name = "London, Greater London" }).ClickAsync();
        }

        public async Task validateCurrentTemperatureWindSpeedAndWeatherDesciptionAreCorrect()
        {
            await Assertions.Expect(_page.Locator("#daylink-0")).ToContainTextAsync("Sunnyyyy and a gentle breeze");
        }

        #endregion
    }
}
