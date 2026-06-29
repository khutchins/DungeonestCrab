using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Actions/Set Tile Type")]
    public class ActionTileNode : ActionProviderNode {
        public Tile TileType;
        public override ITileAction GetAction() => new TileAction(TileType);

        private class TileAction : ITileAction {
            readonly Tile _tile;
            public TileAction(Tile tile) => _tile = tile;
            public void Apply(TileSpec spec, ISeededRandom rand) => spec.Tile = _tile;
        }
    }
}
