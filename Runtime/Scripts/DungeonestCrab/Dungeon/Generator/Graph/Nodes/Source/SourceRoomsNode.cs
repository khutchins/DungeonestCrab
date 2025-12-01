using UnityEngine;
using XNode;
using System.Collections.Generic;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Sources/Rooms")]
    public class SourceRoomsNode : SourceProviderNode {
        public float TriesPerSquare = 0.1f;

        [SerializeReference]
        public List<RoomStrategy> RoomTypes = new List<RoomStrategy>();

        public override ISource GetSource() {
            return new SourceRooms(TriesPerSquare, RoomTypes);
        }
    }
}