namespace BuyOrRent.Web.Settings;

public class ThemeSettings
{
    public const string SectionName = "Theme";

    public string Primary { get; set; } = "#81BDFA";
    public string PrimaryDark { get; set; } = "#5A9BE6";
    public string PrimaryLight { get; set; } = "#B1F0F7";
    public string Secondary { get; set; } = "#FADA7A";
    public string SecondaryDark { get; set; } = "#E8C44A";
    public string Accent { get; set; } = "#B1F0F7";
    public string Background { get; set; } = "#F5F0CD";
    public string BackgroundLight { get; set; } = "#FAF8E8";
    public string Surface { get; set; } = "#FFFFFF";
    public string TextPrimary { get; set; } = "#2D3748";
    public string TextSecondary { get; set; } = "#718096";
    public string TextOnPrimary { get; set; } = "#FFFFFF";
    public string TextOnSecondary { get; set; } = "#2D3748";
    public string Border { get; set; } = "#E2E8F0";
    public string Success { get; set; } = "#48BB78";
    public string Warning { get; set; } = "#FADA7A";
    public string Danger { get; set; } = "#F56565";
    public string Info { get; set; } = "#81BDFA";
    public string NavBackground { get; set; } = "#FFFFFF";
    public string FooterBackground { get; set; } = "#2D3748";
    public string FooterText { get; set; } = "#E2E8F0";
}
