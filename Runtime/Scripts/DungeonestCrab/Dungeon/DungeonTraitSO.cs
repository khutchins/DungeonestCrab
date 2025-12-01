using UnityEngine;
using DungeonestCrab.Dungeon;
using DungeonestCrab.Dungeon.Printer;

namespace DungeonestCrab.Dungeon {
    public abstract class DungeonTraitSO : ScriptableObject {

        public virtual void ModifyTileRules(TileSpec spec, ref TileRuleConfig config) { }

        public virtual void ResolveWallStyle(TileSpec current, TileSpec neighbor, ref WallStyleConfig config) { }

        public virtual void DecorateTile(DungeonPrinter printer, TileSpec spec, Vector3 position, Transform parent) { }

        public virtual void DecorateGlobal(TheDungeon dungeon, DungeonPrinter printer, Transform environmentRoot) { }
    }
}