using DungeonestCrab.Dungeon;
using DungeonestCrab.Dungeon.Printer;
using KH.Audio;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    public abstract class DungeonTraitSO : ScriptableObject {

        public virtual void ModifyTileRules(TileSpec spec, ref TileRuleConfig config) { }

        public virtual void ResolveWallStyle(TileSpec current, TileSpec neighbor, ref WallStyleConfig config) { }

        public virtual void DecorateTile(TheDungeon dungeon, DungeonPrinter printer, IFlatDrawer.FlatInfo info, Vector3 position) { }

        public virtual void DecorateGlobal(TheDungeon dungeon, DungeonPrinter printer, Transform environmentRoot) { }

        public virtual AudioEvent GetFootstepOverride(TileSpec spec) { return null; }

        public virtual T GetMixin<T>(TileSpec spec) where T : TerrainMixin { return null; }
    }
}