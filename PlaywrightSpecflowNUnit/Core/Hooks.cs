using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using BoDi;
using Gherkin.Ast;
using Gherkin.CucumberMessages.Types;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TechTalk.SpecFlow;
using Scenario = AventStack.ExtentReports.Gherkin.Model.Scenario;

namespace PlaywrightSpecflowNUnit.Core
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private static ExtentReports extent;
        private static ExtentTest featurename;
        private static ExtentTest scenarioName;



        public Hooks(IObjectContainer objectContainer, FeatureContext featureContext, ScenarioContext scrnarioContext)
        {
            _featureContext = featureContext;
            _objectContainer = objectContainer;
            _scenarioContext = scrnarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {

            bool isApiTest = _scenarioContext.ScenarioInfo.Tags.Contains("api");

            //Test Report folder initialization
            string folderPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\TestReults\{_scenarioContext.ScenarioInfo.Title}\";
            Directory.CreateDirectory(folderPath);
            string testReportPath = folderPath + "report.html";
            _scenarioContext.Add("extentRepoPath", testReportPath);

            //Extent Report initialization
            extent = new ExtentReports();
            var sparkReporter = new ExtentSparkReporter(_scenarioContext.Get<string>("extentRepoPath"))
            {
                Config = {
                    DocumentTitle = "Automation Report",
                    ReportName = "Automation Report",
                    Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark
                }
            };

            extent.AttachReporter(sparkReporter);
            featurename = extent.CreateTest<AventStack.ExtentReports.Gherkin.Model.Feature>(_featureContext.FeatureInfo.Title);
            scenarioName = featurename.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);

            //API SetUp
            if (isApiTest)
            {
                //setUp
                var playWright = await Playwright.CreateAsync();
                //Context
                var apiContext = await playWright.APIRequest.NewContextAsync();
                //DI
                _objectContainer.RegisterInstanceAs(apiContext, typeof(IAPIRequestContext));
                return;
            }

            //UI SetUp

            //setUp
            var playRyt = await Playwright.CreateAsync();

            //Browser Setup
            var browser = await playRyt.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Channel = "chrome",
                Headless = false,
                SlowMo = 2000,
                Args = new[] { "--start--fullscreen" }
            });

            //Context
            var uiContext = await browser.NewContextAsync();

            await uiContext.Tracing.StartAsync(new TracingStartOptions
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            //page
            var page = await uiContext.NewPageAsync();

            //apiContext
            var apiContextForUI = await playRyt.APIRequest.NewContextAsync();

            //ObjectContainer instantialization for UI
            _objectContainer.RegisterInstanceAs(browser);
            _objectContainer.RegisterInstanceAs(page, typeof(IPage));
            _objectContainer.RegisterInstanceAs(apiContextForUI, typeof(IAPIRequestContext));
        }

        [AfterStep]
        public async Task afterStep()
        {
            bool isApiTest = _scenarioContext.ScenarioInfo.Tags.Contains("api");
            if (!isApiTest)
            {
                var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
                var stepInfo = _scenarioContext.StepContext.StepInfo.Text;
                var resultOfImplementation = _scenarioContext.ScenarioExecutionStatus.ToString();
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var stackTrace = TestContext.CurrentContext.Result.StackTrace;


                var page = _objectContainer.Resolve<IPage>();

                if (_scenarioContext.TestError == null && resultOfImplementation == "OK")
                {
                    if (stepType == "Given")
                    {
                        scenarioName.CreateNode<Given>(stepInfo).Pass(resultOfImplementation);
                    }
                    else if (stepType == "When")
                    {
                        scenarioName.CreateNode<When>(stepInfo).Pass(resultOfImplementation);
                    }
                    else if (stepType == "Then")
                    {
                        scenarioName.CreateNode<Then>(stepInfo).Pass(resultOfImplementation);
                    }
                    else if (stepType == "And" || stepType == "But")
                    {
                        scenarioName.CreateNode<And>(stepInfo).Pass(resultOfImplementation);
                    }
                }

                else if (_scenarioContext.TestError != null || status == TestStatus.Failed)
                {

                    string testError = _scenarioContext.TestError?.Message ?? "Step failed.";
                    byte[] screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        FullPage = true
                    });

                    string base64Screenshot = Convert.ToBase64String(screenshotBytes);

                    if (stepType == "Given")
                    {
                        scenarioName.CreateNode<Given>(stepInfo)
                            .Fail($"{testError}\n Stack Trace :\n{stackTrace}")
                            .AddScreenCaptureFromBase64String(base64Screenshot, "Failure Screenshot");
                    }

                    else if (stepType == "When")
                    {
                        scenarioName.CreateNode<When>(stepInfo)
                            .Fail($"{testError}\n Stack Trace :\n{stackTrace}")
                            .AddScreenCaptureFromBase64String(base64Screenshot, "Failure Screenshot");
                    }

                    else if (stepType == "Then")
                    {
                        scenarioName.CreateNode<Then>(stepInfo)
                            .Fail($"{testError}\n Stack Trace :\n{stackTrace}")
                            .AddScreenCaptureFromBase64String(base64Screenshot, "Failure Screenshot");
                    }

                    else if (stepType == "And" || stepType == "But")
                    {
                        scenarioName.CreateNode<And>(stepInfo)
                            .Fail($"{testError}\n Stack Trace :\n{stackTrace}")
                            .AddScreenCaptureFromBase64String(base64Screenshot, "Failure Screenshot");
                    }
                }

                else if (resultOfImplementation == "StepDefinationPending")
                {

                    string errorMessage = "Step Defination is not implemented!";

                    if (stepType == "Given")
                    {
                        scenarioName.CreateNode<Given>(stepInfo).Skip(errorMessage);
                    }

                    else if (stepType == "When")
                    {
                        scenarioName.CreateNode<When>(stepInfo).Skip(errorMessage);
                    }

                    else if (stepType == "Then")
                    {
                        scenarioName.CreateNode<Then>(stepInfo).Skip(errorMessage);
                    }
                    else if (stepType == "And" || stepType == "But")
                    {
                        scenarioName.CreateNode<And>(stepInfo).Skip(errorMessage);
                    }
                }
            }
        }
        [AfterScenario]
        public async Task afterScenario()
        {
            extent.Flush();

            TestContext.AddTestAttachment(_scenarioContext.Get<string>("extentRepoPath"));

            if (!_scenarioContext.ScenarioInfo.Tags.Contains("api")) {
                var browser = _objectContainer.Resolve<IBrowser>();
                var context = browser.Contexts.FirstOrDefault();

                var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;

                var traceFilePath = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    _scenarioContext.ScenarioInfo.Title,
                    $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                    );

                await context.Tracing.StopAsync(new TracingStopOptions
                {
                    Path = failed ? traceFilePath : null,   
                });

                if (failed && File.Exists(traceFilePath)) {
                    TestContext.AddTestAttachment(traceFilePath, "Playwright Trace");
                }

                await browser.CloseAsync();
            }
        }

    }
}
