using System;
using System.Collections.Generic;
using System.Drawing;
using Windows_form_game_V1._0.Models;

namespace Windows_form_game_V1._0.Services
{
    // Map layout factory - gumagawa ng walls, spawn areas, at NPC positions per map
    internal static class MapLayoutFactory
    {
        public static MapLayoutDefinition Create(string mapKey, bool hasMapImage, Size mapSize)
        {
            if (mapKey == "map2")
            {
                var spawnX = Math.Max(10, (mapSize.Width / 2) - 27);
                var roadWidth = 260;
                var roadLeft = Math.Max(10, (mapSize.Width / 2) - (roadWidth / 2));
                return new MapLayoutDefinition(
                    new List<Rectangle>(),
                    new[]
                    {
                        new Rectangle(roadLeft, 16, roadWidth, Math.Max(120, mapSize.Height - 32))
                    },
                    Rectangle.Empty,
                    Rectangle.Empty,
                    new Point(spawnX, 24),
                    new Point(spawnX, 24),
                    false,
                    false,
                    true);
            }

            if (!hasMapImage)
            {
                return new MapLayoutDefinition(
                    new List<Rectangle>(),
                    new[]
                    {
                        new Rectangle(30, 250, mapSize.Width - 60, 330),
                        new Rectangle(40, 620, mapSize.Width - 80, 160)
                    },
                    new Rectangle(820, 560, 260, 260),
                    new Rectangle(80, 90, 240, 150),
                    new Point(545, 330),
                    new Point(120, 360),
                    true,
                    true,
                    true);
            }

            return new MapLayoutDefinition(
                new[]
                {
                    new Rectangle(15, 110, 215, 205),
                    new Rectangle(250, 130, 180, 165),
                    new Rectangle(600, 100, 260, 190),
                    new Rectangle(124, 548, 205, 124),
                    new Rectangle(700, 500, 270, 190)
                },
                new[]
                {
                    new Rectangle(10, 330, 1060, 95),
                    new Rectangle(530, 0, 95, 840),
                    new Rectangle(835, 250, 230, 170),
                    new Rectangle(360, 430, 250, 280),
                    new Rectangle(885, 680, 170, 120)
                },
                new Rectangle(660, 680, 350, 140),
                new Rectangle(600, 100, 260, 190),
                new Point(545, 330),
                new Point(120, 360),
                true,
                true,
                true);
        }
    }
}
