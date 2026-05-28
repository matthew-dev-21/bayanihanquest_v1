using System;
using System.IO;
using System.Xml.Serialization;
using Windows_form_game_V1._0.Models;

namespace Windows_form_game_V1._0.Services
{
    // Save/Load service - gumagamit ng XML serialization para sa game progress
    internal static class GameProgressStorage
    {
        private const string SaveFileName = "savegame.xml";

        public static string SaveFilePath
        {
            get
            {
                var directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Windows_form_game_V1.0");

                return Path.Combine(directory, SaveFileName);
            }
        }

        public static bool HasSaveFile()
        {
            return File.Exists(SaveFilePath);
        }

        // Save sa AppData/Local folder - gumagawa ng XML file
        public static void Save(GameProgressData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var directory = Path.GetDirectoryName(SaveFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(GameProgressData));
            using (var stream = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(stream, data);
            }
        }

        // Load save file - nagde-deserialize ng XML balik sa game data
        public static bool TryLoad(out GameProgressData data)
        {
            data = null;

            if (!HasSaveFile())
            {
                return false;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(GameProgressData));
                using (var stream = new FileStream(SaveFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    data = serializer.Deserialize(stream) as GameProgressData;
                }

                return data != null;
            }
            catch
            {
                data = null;
                return false;
            }
        }
    }
}
