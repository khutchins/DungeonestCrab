using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Set Terrain")]
    public class TerrainSetterNode : DungeonPassNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public MatcherConnection Matcher;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public MatcherConnection Filter;

        public TerrainSO TerrainToSet;
        public int Range = 0;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            IMatcher matcher = GetInputValue<IMatcher>("Matcher", null);
            if (matcher == null) matcher = TileMatcher.MatchingAll();

            IMatcher filter = GetInputValue<IMatcher>("Filter", null);

            return new TerrainSetter(matcher, filter, Range, TerrainToSet).Modify(dungeon, random);
        }
    }
}