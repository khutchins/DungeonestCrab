using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public interface ITileAction {
        void Apply(TileSpec spec, IRandom rand);
    }
}
