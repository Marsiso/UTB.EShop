using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace UTB.EShop.DistributedServices.WebAPI.Utility;

public static class ExtensionAndMimoTypeManager
{
    public static readonly IReadOnlyDictionary<string, string> MimoTypes = new Dictionary<string, string>()
    {
        { "image/avif", ".avif" },
        { "image/bmp", ".bmp" },
        { "image/gif", ".gif" },
        { "image/vnd.microsoft.icon", ".ico" },
        { "image/jpeg", ".jpg" },
        { "image/png", ".png" },
        { "image/svg+xml", ".svg" },
        { "image/tif", ".tiff" },
        { "image/webp", ".webp" }
    };
    
    public static readonly IReadOnlyDictionary<string, string> Extensions = new Dictionary<string, string>()
    {
        { ".avif", "image/avif" },
        { ".bmp", "image/bmp" },
        { ".gif", "image/gif" },
        { ".ico", "image/vnd.microsoft.icon" },
        { ".jpg", "image/jpeg" },
        { ".png", "image/png" },
        { ".svg", "image/svg+xml" },
        { ".tiff", "image/tif" },
        { ".webp", "image/webp" }
    };

    public static bool TryGetDefaultExtension(this string mimeType, out string? extension) => 
        MimoTypes.TryGetValue(mimeType, out extension);

    public static bool GetMimeTypeFromExtension(this string extension, out string? mimeType) =>
        Extensions.TryGetValue(extension, out mimeType);

    // aspnetcore/src/Middleware/StaticFiles/src/FileExtensionContentTypeProvider.cs is an viable option

    /*public static bool TryGetDefaultExtension(this string mimeType, out string? extension)
    {
        RegistryKey? key;
        object? value;

#pragma warning disable CA1416
        key = Registry.ClassesRoot.OpenSubKey($@"MIME\Database\Content Type\{mimeType}", writable: false);
        value = key?.GetValue("Extension", null);
#pragma warning restore CA1416
        extension = value?.ToString() ;
    
        return value is not null;
    }
    
    public static bool GetMimeTypeFromExtension(this string extension, out string? mimeType)
    {
        RegistryKey? key;
        object? value;

        if (!extension.StartsWith("."))
            extension = "." + extension;
        
#pragma warning disable CA1416
        key = Registry.ClassesRoot.OpenSubKey(extension, false);
        value = key?.GetValue("Content Type", null);
#pragma warning restore CA1416
        mimeType = value?.ToString();

        return value is not null;
    }*/
}