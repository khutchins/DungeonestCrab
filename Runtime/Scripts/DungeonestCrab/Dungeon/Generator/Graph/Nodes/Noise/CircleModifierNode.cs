using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Noise/Modifiers/Circle Modifier")]
    public class CircleModifierNode : NoiseModifierProviderNode {
        public float StartRadius = 20;
        public float EndRadius = 25;
        [Tooltip("The modification amount within the start radius. 0 is no subtraction.")]
        public float MinMod = 0;
        [Tooltip("The modification amount past the end radius. 1 is full subtraction.")]
        public float MaxMod = 1;

        public override INoiseModifier CreateModifier() {
            return new CircleModifier(StartRadius, EndRadius, MinMod, MaxMod);
        }
    }
}
