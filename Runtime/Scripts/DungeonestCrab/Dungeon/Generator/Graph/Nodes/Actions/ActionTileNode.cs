using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Set Tile")]
    public class ActionTileNode : ActionProviderNode {
        public Tile TileType;
        public override ITileAction GetAction() => new TileAction(TileType);

        private class TileAction : ITileAction {
            readonly Tile _tile;
            public TileAction(Tile tile) => _tile = tile;
            public void Apply(TileSpec spec, IRandom rand) => spec.Tile = _tile;
        }
    }
}
