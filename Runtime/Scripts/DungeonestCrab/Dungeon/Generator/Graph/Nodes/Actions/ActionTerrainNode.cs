using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Set Terrain")]
    public class ActionTerrainNode : ActionProviderNode {
        public TerrainSO Terrain;
        public override ITileAction GetAction() => new TerrainAction(Terrain);

        private class TerrainAction : ITileAction {
            readonly TerrainSO _terrain;
            public TerrainAction(TerrainSO terrain) => _terrain = terrain;
            public void Apply(TileSpec spec, IRandom rand) => spec.Terrain = _terrain;
        }
    }
}
