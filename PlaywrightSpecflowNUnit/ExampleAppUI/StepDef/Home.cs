using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaywrightSpecflowNUnit.POM.Pages;
using TechTalk.SpecFlow;

namespace PlaywrightSpecflowNUnit.POM.StepDef
{
    [Binding]
    public class Home
    {
        private HomePage _homePage;
        public Home(HomePage homePage)
        {
            _homePage = homePage;   
        }

        [Given(@"User navigates to bbc weather page")]
        public async Task GivenUserNavigatesToBbcWeatherPage()
        {
            await _homePage.NavigateToApp();
        }

        [When(@"User search for weather in ""([^""]*)"" city")]
        public async Task WhenUserSearchForWeatherInCity(string london)
        {
            await _homePage.SearchWeatherForCity(london);
        }

        [Then(@"Validate current temperature, wind speed and weather desciption are correct")]
        public async Task ThenValidateCurrentTemperatureWindSpeedAndWeatherDesciptionAreCorrect()
        {
            await _homePage.validateCurrentTemperatureWindSpeedAndWeatherDesciptionAreCorrect();
        }

    }
}
