using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Sources/Rooms (BAM-like)")]
    public class SourceBAMRoomsNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;

        [Header("Room Density")]
        [Range(0f, 0.1f)] public float MinDensity = 0.01f;
        [Range(0f, 0.1f)] public float MaxDensity = 0.025f;

        [Min(1)] public int MinRoomSize = 3;
        [Min(1)] public int MaxRoomSize = 9;

        [Header("Advanced")]
        public int PlacementAttempts = 10;
        public bool ForceOddAlignment = true;

        public override ISource GetSource() {
            return new SourceBAMRooms(TileToSet, MinDensity, MaxDensity, MinRoomSize, MaxRoomSize, PlacementAttempts, ForceOddAlignment);
        }
    }
}