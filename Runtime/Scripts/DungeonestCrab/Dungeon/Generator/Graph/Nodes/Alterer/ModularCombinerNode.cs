using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Connect Regions (Modular)")]
    public class ModularCombinerNode : DungeonPassNode {

        [Header("Components")]
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public ConnectorConnection Connector;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public PathfinderConnection Pathfinder;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public CarverConnection Carver;

        [Header("Defaults")]
        [Tooltip("Used if no specific Carver is connected.")]
        public TerrainSO DefaultTerrain;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {

            IRegionConnector connector = GetInputValue<IRegionConnector>("Connector", null);
            IPathFinder pathfinder = GetInputValue<IPathFinder>("Pathfinder", null);
            ITileCarver carver = GetInputValue<ITileCarver>("Carver", null);

            connector ??= new ConnectorRandom(0.1f);
            pathfinder ??= new PathfinderDogleg();
            carver ??= new CarverStandard(Tile.Floor, DefaultTerrain);

            return new ModularCombiner(connector, pathfinder, carver).Modify(dungeon, random);
        }
    }
}