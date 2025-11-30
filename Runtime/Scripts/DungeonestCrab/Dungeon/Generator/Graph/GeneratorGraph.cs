using DungeonestCrab.Dungeon;
using Pomerandomian;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateAssetMenu(fileName = "New Generator Graph", menuName = "DungeonestCrab/Generator Graph")]
    public class GeneratorGraph : NodeGraph {
        [Header("Global Configuration")]
        public int Width = 40;
        public int Height = 40;

        [Header("Generation Settings")]
        [Tooltip("How many times to retry if a node returns failure.")]
        public int MaxAttempts = 20;

        public TheDungeon Generate(int seed) {
            return Generate(new SystemRandom(seed));
        }

        public TheDungeon Generate(IRandom rng) {
            StartNode start = nodes.FirstOrDefault(x => x is StartNode) as StartNode;
            for (int i = 0; i < MaxAttempts; i++) {
                TheDungeon dungeon = new TheDungeon(Width, Height, rng);

                bool success = start.GenerateRuntime(rng, dungeon);

                if (success) {
                    Debug.Log($"Dungeon generated successfully on attempt {i + 1}");
                    dungeon.UpdateDungeonComputations();
                    return dungeon;
                } else {
                    Debug.LogWarning($"Attempt {i + 1} failed. Retrying...");
                }
            }

            Debug.LogError($"Dungeon generation failed after {MaxAttempts} attempts.");
            return null;
        }
    }
}