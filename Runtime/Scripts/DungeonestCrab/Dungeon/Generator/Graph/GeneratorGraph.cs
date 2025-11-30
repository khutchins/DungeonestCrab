using DungeonestCrab.Dungeon;
using Pomerandomian;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateAssetMenu(fileName = "New Generator Graph", menuName = "DungeonestCrab/Generator Graph")]
    public class GeneratorGraph : NodeGraph {
        public TheDungeon Generate(int seed) {
            return Generate(new SystemRandom(seed));
        }

        public TheDungeon Generate(IRandom rng) {
            StartNode start = nodes.FirstOrDefault(x => x is StartNode) as StartNode;
            if (start == null) {
                Debug.LogError("No StartNode found in GeneratorGraph!");
                return null;
            }

            return start.GenerateRuntime(rng);
        }
    }
}