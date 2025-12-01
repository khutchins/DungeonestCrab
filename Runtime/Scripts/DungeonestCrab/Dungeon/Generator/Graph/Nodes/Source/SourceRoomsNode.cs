using UnityEngine;
using XNode;
using System.Collections.Generic;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Sources/Rooms (Strategies)")]
    public class SourceRoomsNode : SourceProviderNode {
        public float TriesPerSquare = 0.1f;

        // ODIN MAGIC: This allows you to pick "BasicRoom", "RoundedRoom", etc from a dropdown
        [SerializeReference]
        public List<RoomStrategy> RoomTypes = new List<RoomStrategy>();

        public override ISource GetSource() {
            // Convert the serialized strategies into the runtime SourceRooms class
            // You might need to update SourceRooms to accept RoomStrategy directly
            return new SourceRooms(TriesPerSquare, RoomTypes);
        }
    }
}