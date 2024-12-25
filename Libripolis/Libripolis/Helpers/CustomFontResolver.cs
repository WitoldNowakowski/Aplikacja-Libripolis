using PdfSharpCore.Fonts;
using System;
using System.IO;

public class CustomFontResolver : IFontResolver
{
    public string DefaultFontName => "Arial"; // Domyślna nazwa czcionki

    public byte[] GetFont(string faceName)
    {
        string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/Arial.ttf");

        if (!File.Exists(fontPath))
        {
            throw new FileNotFoundException("Czcionka nie została znaleziona w podanej ścieżce.", fontPath);
        }

        return File.ReadAllBytes(fontPath);
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
        {
            return new FontResolverInfo("Arial");
        }

        return null; // Jeśli brak czcionki, zwróć null
    }
}
