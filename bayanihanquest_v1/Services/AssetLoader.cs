using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Windows_form_game_V1._0.Services
{
    // Simple asset loader utility that centralizes asset path search and image loading.
    // Asset loader - naghahanap ng images sa assets folder at parent directories
    internal static class AssetLoader
    {
        public static Image LoadImage(params string[] fileNames)
        {
            var path = FindAssetPath(fileNames);
            if (string.IsNullOrWhiteSpace(path)) return null;

            try
            {
                return Image.FromFile(path);
            }
            catch
            {
                return null;
            }
        }

        public static string FindAssetPath(string[] fileNames)
        {
            var startup = Application.StartupPath;
            var candidates = fileNames
                .SelectMany(fn => new[]
                {
                    Path.Combine(startup, fn),
                    Path.Combine(startup, "assets", fn),
                    Path.Combine(startup, "assets", "wasdControllerimage", fn)
                })
                .ToList();

            var current = new DirectoryInfo(startup);
            for (var i = 0; i < 6 && current != null; i++)
            {
                foreach (var fn in fileNames)
                {
                    candidates.Add(Path.Combine(current.FullName, fn));
                    candidates.Add(Path.Combine(current.FullName, "assets", fn));
                    candidates.Add(Path.Combine(current.FullName, "assets", "wasdControllerimage", fn));
                }

                current = current.Parent;
            }

            return candidates.FirstOrDefault(File.Exists);
        }
    }
}
