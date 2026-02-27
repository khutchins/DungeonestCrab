using UnityEngine;
using DungeonestCrab.Dungeon.Generator;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Pathfinding/Carvers/Directional")]
    public class CarverDirectionalNode : CarverProviderNode {

        [Header("Defaults")]
        public Tile TileType = Tile.Floor;
        public TerrainSO Terrain;
        [Tooltip("If true, existing floors will not have their terrain changed when carving.")]
        public bool PreserveExistingFloors = true;

        public override ITileCarver GetCarver() {
            return new CarverDirectional(TileType, Terrain, PreserveExistingFloors);
        }
    }
}
