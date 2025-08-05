using System.Reflection;

namespace Cucumber.HtmlFormatter;

/// <summary>
/// Default implementation of IResourceProvider
/// </summary>
public class DefaultResourceProvider : IResourceProvider
{
    private const string TEMPLATE_RESOURCE_NAME = "index.mustache.html";
    private const string CSS_RESOURCE_NAME = "main.css";
    private const string JAVASCRIPT_RESOURCE_NAME = "main.js";
    private readonly Assembly _assembly;
    private readonly string _resourceNamespace;

    public DefaultResourceProvider()
    {
        _assembly = typeof(MessagesToHtmlWriter).Assembly;
        _resourceNamespace = "Cucumber.HtmlFormatter.Resources.";
    }

    /// <summary>
    /// Constructor with custom assembly and namespace
    /// </summary>
    /// <param name="assembly">The assembly to load resources from</param>
    /// <param name="resourceNamespace">The namespace prefix for resources</param>
    public DefaultResourceProvider(Assembly assembly, string resourceNamespace)
    {
        _assembly = assembly;
        _resourceNamespace = resourceNamespace;
    }

    /// <inheritdoc />
    public string GetTemplateResource()
    {
        return GetResource(TEMPLATE_RESOURCE_NAME);
    }

    /// <inheritdoc />
    public string GetCssResource()
    {
        return GetResource(CSS_RESOURCE_NAME);
    }

    /// <inheritdoc />
    public string GetJavaScriptResource()
    {
        return GetResource(JAVASCRIPT_RESOURCE_NAME);
    }

    /// <summary>
    /// Gets a resource from the assembly
    /// </summary>
    /// <param name="name">The resource name</param>
    /// <returns>The resource content</returns>
    protected string GetResource(string name)
    {
        var resourceStream = _assembly.GetManifestResourceStream(_resourceNamespace + name);
        if (resourceStream == null)
            throw new InvalidOperationException($"Resource '{name}' not found in assembly '{_assembly.FullName}'");

        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }
}