using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class EntityPassNode : DungeonPassNode {
        [Header("Inputs")]
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public EntitySourceConnection Entities;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public MatcherConnection Matcher;

        [Header("Common")]
        public bool AvoidBlockingPath = true;

        protected EntitySource GetEntitySource() {
            return GetInputValue<EntitySource>("Entities", null);
        }

        protected IMatcher GetMatcher() {
            var m = GetInputValue<IMatcher>("Matcher", null);
            // Default to floor if unconnected
            return m ?? TileMatcher.Matching(Tile.Floor);
        }
    }
}