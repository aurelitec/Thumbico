using System.Diagnostics;
using System.Text.Json;

namespace Thumbico.Settings
{
    //internal class AppSettings
    //{
    //    public Color BackColor { get; set; }

    //    public static AppSettings Load()
    //    {
    //        string jsonString = File.ReadAllText(path);

    //        var deserializeOptions = new JsonSerializerOptions
    //        {
    //            ReadCommentHandling = JsonCommentHandling.Skip,
    //            AllowTrailingCommas = true,
    //        };
    //        return JsonSerializer.Deserialize<AppSettings>(jsonString, deserializeOptions);
    //    }
    //}

    /// <summary>
    /// The App Settings class.
    /// </summary>
    internal class AppSettings
    {
        /// <summary>
        /// Gets or sets the value of the background color setting.
        /// </summary>
        public Color PreviewBackColor { get; set; } = Color.White;

        /// <summary>
        /// Gets the default settings file. Handles portable and installable scenarios.
        /// </summary>
        private static string DefaultSettingsFilePath
        {
            get
            {
                string path = Path.ChangeExtension(Application.ExecutablePath, ".settings.json");

                if (!File.Exists(path))
                {
                    path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        Application.CompanyName,
                        Application.ProductName,
                        Path.GetFileName(path));
                }

                return path;
            }
        }

        /// <summary>
        /// Loads the app settings from the default settings file.
        /// </summary>
        /// <returns>An AppSettings instance with the loaded values.</returns>
        public static AppSettings Load()
        {
            try
            {
                string jsonString = File.ReadAllText(DefaultSettingsFilePath);

                var deserializeOptions = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                };
                return JsonSerializer.Deserialize<AppSettings>(jsonString, deserializeOptions);
            }
            catch (Exception e)
            {
                LogError("Loading settings", e.Message);

                // Return an AppSettings instace with default values
                return new AppSettings();
            }
        }

        /// <summary>
        /// Saves the app settings to the default settings file.
        /// </summary>
        public void Save()
        {
            try
            {
                File.WriteAllText(DefaultSettingsFilePath, JsonSerializer.Serialize(this));
            }
            catch (Exception e)
            {
                LogError("Saving settings", e.Message);
            }
        }

        /// <summary>
        /// Logs an error message to the Windows Event Log.
        /// </summary>
        private static void LogError(string category, string message)
        {
            try
            {
                EventLog.WriteEntry("Application", $"{Application.ProductName}: {category}: {message}", EventLogEntryType.Error);
            }
            catch
            {
                // Ignore exceptions while writing to the Event Log
            }
        }
    }

}
