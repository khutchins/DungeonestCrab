using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
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