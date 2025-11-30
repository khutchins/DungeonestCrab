using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Prune Dead Ends")]
    public class DeadEndPrunerNode : DungeonPassNode {
        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return new DeadEndPruner().Modify(dungeon, random);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Set Terrain")]
    public class TerrainSetterNode : DungeonPassNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public MatcherConnection Matcher;

        public TerrainSO TerrainToSet;
        public int Range = 0;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            IMatcher matcher = GetInputValue<IMatcher>("Matcher", null);
            if (matcher == null) matcher = TileMatcher.MatchingAll();

            return new TerrainSetter(matcher, Range, TerrainToSet).Modify(dungeon, random);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Connect Regions (Simple)")]
    public class AdjacentCombinerNode : DungeonPassNode {
        public TerrainSO Terrain;
        [Range(0, 1)] public float ExtraJunctionChance = 0.1f;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return new AdjacentCombiner(Terrain, ExtraJunctionChance).Modify(dungeon, random);
        }
    }
}