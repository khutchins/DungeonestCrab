using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Connector: Random")]
    public class ConnectorRandomNode : ConnectorProviderNode {
        [Range(0, 1)] public float ExtraConnectionChance = 0.0f;
        public override IRegionConnector GetConnector() {
            return new ConnectorRandom(ExtraConnectionChance);
        }
    }
}