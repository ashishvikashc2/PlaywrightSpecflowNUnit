using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace PlaywrightSpecflowNUnit.ExampleAppAPI.Services
{
    public class HerokuAppServices
    {
        private IAPIRequestContext _context;
        public HerokuAppServices(IAPIRequestContext aPIRequestContext)
        {
            _context = aPIRequestContext;   
        }

        public async Task VerifyGetBookingsAsync()
        {
            var response = await _context.GetAsync("https://restful-booker.herokuapp.com/booking");
            Assert.That(response.Status, Is.EqualTo(200), "Expected status code to be 200");
            
            var contextText = await response.TextAsync();
            var content = JToken.Parse(contextText);

            Assert.That(content.Type, Is.EqualTo(JTokenType.Array), "Expect content type to be array");

            var bookings = (JArray)content;

            Assert.That(bookings.Count, Is.GreaterThan(0), "Expect at lease 1 booking");
        }

        public async Task VerifyGetBookingsService()
        {
            var response = await _context.GetAsync("https://restful-booker.herokuapp.com/booking");
            Assert.That(response.Status, Is.EqualTo(200), "Expected status code to be 200");

            var contextText = await response.TextAsync();
            var content = JToken.Parse(contextText);

            Assert.That(content.Type, Is.EqualTo(JTokenType.Array), "Expect content type to be array");

            var bookings = (JArray)content;

            Assert.That(bookings.Count, Is.GreaterThan(0), "Expect at lease 1 booking");
        }
    }
}
