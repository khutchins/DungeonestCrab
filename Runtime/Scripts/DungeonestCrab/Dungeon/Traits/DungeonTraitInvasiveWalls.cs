using UnityEngine;
using DungeonestCrab.Dungeon.Printer;

namespace DungeonestCrab.Dungeon {

    [CreateAssetMenu(menuName = "Dungeon/Traits/Invasive Walls")]
    public class DungeonTraitInvasiveWalls : DungeonTraitSO {

        public override void ResolveWallStyle(TileSpec current, TileSpec neighbor, ref WallStyleConfig config) {
            // Invasive Logic: 
            // We want the Current tile (the floor we are standing on) to provide the wall texture,
            // rather than the Neighbor tile (the wall/void).
            config.StyleSource = current;
        }
    }
}