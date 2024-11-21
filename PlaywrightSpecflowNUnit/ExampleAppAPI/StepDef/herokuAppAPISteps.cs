using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaywrightSpecflowNUnit.ExampleAppAPI.Services;
using TechTalk.SpecFlow;

namespace PlaywrightSpecflowNUnit.ExampleAppAPI.StepDef
{
    [Binding]
    public class herokuAppAPISteps
    {
        private readonly HerokuAppServices _herokuAppServices;
        public herokuAppAPISteps(HerokuAppServices herokuAppServices)
        {
            _herokuAppServices = herokuAppServices;
        }

        [Given(@"I have initialized the API context")]
        public async Task GivenIHaveInitializedTheAPIContext()
        {
            //Initalize if needed
        }

        [When(@"I request the list of bookings")]
        public async Task WhenIRequestTheListOfBookings()
        {
            await _herokuAppServices.VerifyGetBookingsService();
        }

        [Then(@"the response should contain a list of bookings")]
        public async Task ThenTheResponseShouldContainAListOfBookings()
        {
            await _herokuAppServices.VerifyGetBookingsAsync();
        }

    }
}
