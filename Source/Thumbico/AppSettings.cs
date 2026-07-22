// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thumbico;

/// <summary>
/// The choices the user made, carried between runs.
/// </summary>
internal sealed class AppSettings
{
    public Rectangle WindowBounds { get; set; }

    public bool WindowMaximized { get; set; }

    /// <summary>Gets or sets the solid canvas colour, or null for the checkerboard.</summary>
    public Color? BackgroundColor { get; set; }

    public string SizeSelection { get; set; } = string.Empty;

    public ThumbicoSource Source { get; set; } = ThumbicoSource.Auto;

    public ThumbicoOptions Options { get; set; } = ThumbicoOptions.None;
}

/// <summary>
/// Writes a colour as an ARGB hex string.
/// </summary>
/// <remarks>
/// System.Text.Json has no built-in converter for Color. Without this it serializes the struct's
/// read-only surface, reads nothing back, and raises no error, which is how the 2021 build lost the
/// only value it persisted.
/// </remarks>
internal sealed class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? text = reader.GetString();

        return string.IsNullOrWhiteSpace(text)
            || !int.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int argb)
                ? Color.Empty
                : Color.FromArgb(argb);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToArgb().ToString("X8", CultureInfo.InvariantCulture));
}

/// <summary>
/// Loads and saves the settings file.
/// </summary>
/// <remarks>
/// Portable first: the file sits beside the executable so a portable copy carries its settings with
/// it. An installed copy lives under Program Files and cannot write there, so that case falls back
/// to the per-user location rather than failing.
/// </remarks>
internal sealed class SettingsStore
{
    private const string FileName = "Thumbico.settings.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new ColorJsonConverter() },
    };

    internal SettingsStore(string directory)
    {
        this.FilePath = Path.Combine(directory, FileName);
    }

    internal string FilePath { get; }

    /// <summary>
    /// Chooses the settings location, preferring the executable's own directory.
    /// </summary>
    internal static SettingsStore CreateDefault()
    {
        string beside = AppContext.BaseDirectory;

        return new SettingsStore(IsWritable(beside) ? beside : PerUserDirectory());
    }

    /// <summary>
    /// Reads the settings, falling back to defaults for a missing, unreadable, or corrupt file.
    /// </summary>
    internal AppSettings Load()
    {
        try
        {
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(this.FilePath), SerializerOptions)
                ?? new AppSettings();
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException)
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// Writes the settings. Failing to save is not worth interrupting the user over.
    /// </summary>
    internal void Save(AppSettings settings)
    {
        try
        {
            File.WriteAllText(this.FilePath, JsonSerializer.Serialize(settings, SerializerOptions));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
        }
    }

    private static bool IsWritable(string directory)
    {
        string probe = Path.Combine(directory, $"{FileName}.{Guid.NewGuid():N}");

        try
        {
            File.WriteAllBytes(probe, []);
            File.Delete(probe);
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static string PerUserDirectory()
    {
        string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aurelitec", "Thumbico");
        Directory.CreateDirectory(directory);

        return directory;
    }
}
