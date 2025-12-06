using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Sources/Rooms (MD-Like)")]
    public class SourceMDRoomsNode : SourceProviderNode {

        [Header("General")]
        public Tile TileToSet = Tile.Floor;

        [Header("Grid Configuration")]
        [Min(2)] public int Columns = 4;
        [Min(2)] public int Rows = 3;

        [Header("Room Logic")]
        [Range(0, 1)] public float RoomDensity = 0.7f;

        [Tooltip("How many connections will be attempted per room. Unconnected rooms will be removed.")]
        [Range(0, 2)] public float Connectivity = 1;

        [Tooltip("If false, all dead-end corridors will be forced to connect to a neighbor.")]
        public bool AllowDeadEnds = true;

        [Header("Juice")]
        [Range(0, 1)] public float ImperfectionChance = 0.4f;
        [Range(0, 1)] public float MergeChance = 0.05f;

        public override ISource GetSource() {
            return new SourceMDRooms(TileToSet, Columns, Rows, RoomDensity, Connectivity, AllowDeadEnds, ImperfectionChance, MergeChance);
        }
    }
}