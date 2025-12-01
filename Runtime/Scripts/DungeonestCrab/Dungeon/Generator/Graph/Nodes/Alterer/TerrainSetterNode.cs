using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
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
}