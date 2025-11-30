using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Stamper")]
    public class StamperNode : DungeonPassNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public SourceConnection Source;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public BoundsConnection Bounds;

        public TerrainSO Terrain;
        public bool PassTerrains;

        protected override void ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            ISource source = GetInputValue<ISource>("Source", null);
            Bounds boundsInput = GetInputValue<Bounds>("Bounds", null);

            if (source == null) return;
            boundsInput ??= new FullBounds();

            Stamper stamper = new Stamper(source, Terrain, PassTerrains, boundsInput);
            stamper.Modify(dungeon, random);
        }
    }
}