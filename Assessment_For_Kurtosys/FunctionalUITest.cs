using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;

namespace Assessment_For_Kurtosys
{
  [TestClass]
  public class FunctionalUITest
  {
    private EdgeDriver _driver;

    [TestInitialize]
    public void EdgeDriverInitialize()
    {
      // Initialize EdgeDriver with options  
      var options = new EdgeOptions
      {
        PageLoadStrategy = PageLoadStrategy.Normal
      };
      _driver = new EdgeDriver(options);

      // Set the window size to full screen  
      _driver.Manage().Window.Maximize();
    }

    [TestMethod]
    public void KurtosysWhitePaperTest_FunctionalUITest()
    {
      // Step 1: Go to https://www.kurtosys.com/  
      _driver.Navigate().GoToUrl("https://www.kurtosys.com/");
      Assert.IsTrue(_driver.Title.Contains("Kurtosys"), "Kurtosys homepage did not load correctly.");


      // Note, I have had difficulty in actioning the hover and click over the "Insights" button. The test fails at this step.
      // Step 2: Navigate to “INSIGHTS” (instead of RESOURCES)  
      WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20)); // Increased timeout
      Actions actions = new Actions(_driver);

      // Locate the "Insights" element
      IWebElement insightsButton = wait.Until(driver => driver.FindElement(By.XPath("//div[@class='kurtosys-toggle__label-text' and contains(text(), 'Insights')]/preceding-sibling::div")));

      // Hover over the "Insights" element
      actions.MoveToElement(insightsButton).Perform();

      // Step 3: Locate and click on the "White Papers & eBooks" span element
      IWebElement whitePapersLink = wait.Until(driver => driver.FindElement(By.XPath("//span[@class='elementor-icon-list-text' and contains(text(), 'White Papers & eBooks')]")));
      whitePapersLink.Click();

      // Step 4: Verify the Title reads “White Papers”  
      Assert.IsTrue(_driver.Title.Contains("White Papers"), "Title does not read 'White Papers'");

      // Step 5: Click on any white paper tile: UCITS Whitepaper  
      IWebElement ucitsWhitePaperLink = wait.Until(driver => driver.FindElement(By.XPath("//div[@class='elementor-post__card']//a[contains(text(), 'UCITS White Paper')]")));
      ucitsWhitePaperLink.Click();

      // Step 6-9: Fill in Fields for “First Name”, “Last Name”, “Company”, “Industry”  
      _driver.FindElement(By.Id("18882_231669pi_18882_231669")).SendKeys("Rhyne"); // Firstname
      _driver.FindElement(By.Id("18882_231671pi_18882_231671")).SendKeys("Killian"); // Lastname
      _driver.FindElement(By.Id("18882_231675pi_18882_231675")).SendKeys("Kurtosys Assessment"); // Company
      _driver.FindElement(By.Id("18882_231677pi_18882_231677")).SendKeys("Website & Reporting Services"); // Industry

      // Step 10: DO NOT populate the "Email” field  
      // No Action to take place. The email field will be left blank.  

      // Step 11: Click “Send me a copy”  
      IWebElement sendButton = _driver.FindElement(By.XPath("//input[@type='submit' and @value='Send me a copy']"));
      sendButton.Click();

      // Step 12: Add screenshot of the error messages  
      Screenshot screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
      string screenshotPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_Screenshot.png");
      screenshot.SaveAsFile(screenshotPath);
      Console.WriteLine("Screenshot saved at: " + screenshotPath);

      // Step 13: Validate all error messages
      WebDriverWait waitForError = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
      IWebElement emailErrorMessage = waitForError.Until(driver => driver.FindElement(By.XPath("//p[@class='error no-label' and contains(text(), 'This field is required.')]")));
      Assert.IsTrue(emailErrorMessage.Displayed, "Email error message not displayed.");
    }

    [TestCleanup]
    public void EdgeDriverCleanup()
    {
      _driver.Quit();
    }
  }
}
