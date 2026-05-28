using System.Collections.Generic;
using System.Drawing;

namespace Windows_form_game_V1._0.Models
{
    internal sealed class MapLayoutDefinition
    {
        public MapLayoutDefinition(
            IEnumerable<Rectangle> wallBounds,
            IEnumerable<Rectangle> trashSpawnAreas,
            Rectangle storeInteractionZone,
            Rectangle hallBounds,
            Point playerSpawnWithMapImage,
            Point playerSpawnFallback,
            bool hasCaptainNpc,
            bool hasStore,
            bool allowTrash)
        {
            WallBounds = new List<Rectangle>(wallBounds);
            TrashSpawnAreas = new List<Rectangle>(trashSpawnAreas);
            StoreInteractionZone = storeInteractionZone;
            HallBounds = hallBounds;
            PlayerSpawnWithMapImage = playerSpawnWithMapImage;
            PlayerSpawnFallback = playerSpawnFallback;
            HasCaptainNpc = hasCaptainNpc;
            HasStore = hasStore;
            AllowTrash = allowTrash;
        }

        public List<Rectangle> WallBounds { get; private set; }

        public List<Rectangle> TrashSpawnAreas { get; private set; }

        public Rectangle StoreInteractionZone { get; private set; }

        public Rectangle HallBounds { get; private set; }

        public Point PlayerSpawnWithMapImage { get; private set; }

        public Point PlayerSpawnFallback { get; private set; }

        public bool HasCaptainNpc { get; private set; }

        public bool HasStore { get; private set; }

        public bool AllowTrash { get; private set; }
    }
}
