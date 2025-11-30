using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

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

    [CreateNodeMenu("Dungeon/Actions/Add Entities (Percent)")]
    public class AddEntityPercentNode : EntityPassNode {
        [Range(0, 1)] public float Percent = 0.1f;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            EntitySource source = GetEntitySource();
            if (source == null) return false;

            return new AddEntityPercent(source, GetMatcher(), AvoidBlockingPath, Percent)
                .Modify(dungeon, random);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Add Entities (Count)")]
    public class AddEntityCountNode : EntityPassNode {
        public int MinCount = 1;
        public int TargetCount = 5;
        [Range(0, 1)] public float Chance = 1f;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            EntitySource source = GetEntitySource();
            return new AddEntity(source, GetMatcher(), AvoidBlockingPath, MinCount, TargetCount, Chance)
                .Modify(dungeon, random);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Add Entities (Path)")]
    public class AddEntityPathNode : EntityPassNode {
        [Header("Path Settings")]
        public Vector2Int StartOffset;
        public bool SetAngles = true;
        public float YRotation = 0;

        [Header("Destination")]
        public bool ToFarthestPoint = true;
        public Vector2Int SpecificDestination;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            EntitySource source = GetEntitySource();
            if (source == null) return false;

            // Ideally this would come from some finder logic.
            Vector2Int start = new Vector2Int(1, 1);

            AddEntitiesAlongPath.Builder builder;

            if (ToFarthestPoint) {
                builder = AddEntitiesAlongPath.Builder.ToFarthestPoint(source, start);
            } else {
                builder = AddEntitiesAlongPath.Builder.ForShortestPath(source, start, SpecificDestination);
            }

            return builder.SetMatcher(GetMatcher())
                   .SetAngles(SetAngles, StartOffset)
                   .SetAdditionalYRotation(YRotation)
                   .Build()
                   .Modify(dungeon, random);
        }
    }
}