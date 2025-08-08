using Cucumber.HtmlFormatter;

namespace Cucumber.HtmlFormatterTest;

[TestClass]
public class DefaultResourceProviderTest
{
    private DefaultResourceProvider _resourceProvider = null!;

    [TestInitialize]
    public void Setup()
    {
        _resourceProvider = new DefaultResourceProvider();
    }

    [TestMethod]
    public void GetTemplateResource_ReturnsNonEmptyString()
    {
        // Act
        var template = _resourceProvider.GetTemplateResource();
        
        // Assert
        Assert.IsNotNull(template, "Template resource should not be null");
        Assert.IsTrue(!string.IsNullOrWhiteSpace(template), "Template resource should not be empty");
        Assert.IsTrue(template.Contains("{{css}}"), "Template should contain CSS placeholder");
        Assert.IsTrue(template.Contains("{{messages}}"), "Template should contain messages placeholder");
        Assert.IsTrue(template.Contains("{{script}}"), "Template should contain script placeholder");
    }

    [TestMethod]
    public void GetCssResource_ReturnsNonEmptyString()
    {
        // Act
        var css = _resourceProvider.GetCssResource();
        
        // Assert
        Assert.IsNotNull(css, "CSS resource should not be null");
        Assert.IsTrue(!string.IsNullOrWhiteSpace(css), "CSS resource should not be empty");
    }

    [TestMethod]
    public void GetJavaScriptResource_ReturnsNonEmptyString()
    {
        // Act
        var javascript = _resourceProvider.GetJavaScriptResource();
        
        // Assert
        Assert.IsNotNull(javascript, "JavaScript resource should not be null");
        Assert.IsTrue(!string.IsNullOrWhiteSpace(javascript), "JavaScript resource should not be empty");
    }

    [TestMethod]
    public void AllResources_LoadSuccessfully()
    {
        // This test verifies that all resources can be loaded in sequence without errors
        
        // Act & Assert - if any of these throw an exception, the test will fail
        var template = _resourceProvider.GetTemplateResource();
        Assert.IsNotNull(template, "Template resource should not be null");

        var css = _resourceProvider.GetCssResource();
        Assert.IsNotNull(css, "CSS resource should not be null");

        var javascript = _resourceProvider.GetJavaScriptResource();
        Assert.IsNotNull(javascript, "JavaScript resource should not be null");
    }
}