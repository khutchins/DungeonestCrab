using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Connect Regions (Simple)")]
    public class AdjacentCombinerNode : DungeonPassNode {
        public TerrainSO Terrain;
        [Range(0, 1)] public float ExtraJunctionChance = 0.1f;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return new AdjacentCombiner(Terrain, ExtraJunctionChance).Modify(dungeon, random);
        }
    }
}