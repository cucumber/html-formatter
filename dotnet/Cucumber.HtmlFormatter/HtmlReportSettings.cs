namespace Cucumber.HtmlFormatter;

/// <summary>
/// Settings for HTML report generation
/// </summary>
public class HtmlReportSettings
{
    private const string DEFAULT_TITLE = "Cucumber";
    private string? _icon = null;

    /// <summary>
    /// Gets or sets the title of the HTML report.
    /// Default is "Cucumber".
    /// </summary>
    public string Title { get; set; } = DEFAULT_TITLE;

    /// <summary>
    /// Gets or sets the icon for the HTML report.
    /// Default is the Cucumber icon as a base64-encoded SVG.
    /// </summary>
    public string Icon
    {
        get => _icon ?? LoadDefaultIcon();
        set => _icon = value;
    }

    /// <summary>
    /// Gets or sets custom CSS to include in the HTML report.
    /// Default is empty.
    /// </summary>
    public string CustomCss { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets custom Javascript to include after the main Javascript section of the HTML report.
    /// Default is empty.
    /// </summary>
    public string CustomScript { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new instance of HtmlReportSettings with default values.
    /// </summary>
    public HtmlReportSettings()
    {
    }

    private static string LoadDefaultIcon()
        => MessagesToHtmlWriter.GetResource("icon.url");
}