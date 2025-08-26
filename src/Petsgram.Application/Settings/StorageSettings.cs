namespace Petsgram.Application.Settings;

public class StorageSettings
{
    public const string SectionName = "Storage";
    public string PhotoPhysicalPath { get; set; } = string.Empty;
    public string PhotoPublicPath { get; set; } = string.Empty;
}
