using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Connect Regions (Modular)")]
    public class ModularCombinerNode : DungeonPassNode {

        [Header("Components")]
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public ConnectorConnection Connector;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public PathfinderConnection Pathfinder;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public CarverConnection Carver;

        [Header("Defaults")]
        [Tooltip("Used if no specific Carver is connected.")]
        public TerrainSO DefaultTerrain;
        [Tooltip("If true, existing floors will not have their terrain changed when carving.")]
        public bool PreserveExistingFloors = true;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {

            IRegionConnector connector = GetInputValue<IRegionConnector>("Connector", null);
            IPathFinder pathfinder = GetInputValue<IPathFinder>("Pathfinder", null);
            ITileCarver carver = GetInputValue<ITileCarver>("Carver", null);

            connector ??= new ConnectorRandom(0.1f);
            pathfinder ??= new PathfinderDogleg();
            carver ??= new CarverStandard(Tile.Floor, DefaultTerrain, PreserveExistingFloors);

            return new ModularCombiner(connector, pathfinder, carver).Modify(dungeon, random);
        }
    }
}