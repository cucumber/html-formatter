namespace Cucumber.HtmlFormatter;

/// <summary>
/// Interface for providing resources to the HTML formatter.
/// This is an experimental API for allowing customizations, will be replaced by a more flexible solution in an upcoming release.
/// </summary>
public interface IResourceProvider
{
    /// <summary>
    /// Gets the HTML template
    /// </summary>
    /// <returns>The HTML template</returns>
    string GetTemplateResource();
    
    /// <summary>
    /// Gets the CSS resource
    /// </summary>
    /// <returns>The CSS resource</returns>
    string GetCssResource();
    
    /// <summary>
    /// Gets the JavaScript resource
    /// </summary>
    /// <returns>The JavaScript resource</returns>
    string GetJavaScriptResource();
}