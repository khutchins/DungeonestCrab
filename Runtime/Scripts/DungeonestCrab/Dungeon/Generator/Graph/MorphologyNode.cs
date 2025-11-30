using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;
using System.Collections.Generic;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Morphology (Smooth)")]
    public class MorphologyNode : DungeonPassNode {
        public Morphology.Operation Operation;
        public int Iterations = 1;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            Morphology morph = new Morphology(Operation, Iterations);
            return morph.Modify(dungeon, random);
        }
    }
}