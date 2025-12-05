using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Connector: MST")]
    public class ConnectorMSTNode : ConnectorProviderNode {
        public bool UseCentroids = true;
        [Range(0, 1)] public float LoopChance = 0.0f;
        public override IRegionConnector GetConnector() {
            return new ConnectorMST(UseCentroids, LoopChance);
        }
    }
}