using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Noise/Modifiers/Smoothstep Edge Mask")]
    public class SmoothStepModifierNode : NoiseModifierProviderNode {
        [Tooltip("The distance from the edge where the modification starts to be at MinMod.")]
        public int InsetWidth = 0;
        [Tooltip("The length of the gradient from MinMod to MaxMod.")]
        public int GradientLength = 10;
        
        [Tooltip("The modification amount at the very edge/inset. 0 is no subtraction.")]
        public float MinMod = 1;
        [Tooltip("The modification amount in the center area. Usually 0 (no subtraction).")]
        public float MaxMod = 0;

        public override INoiseModifier CreateModifier() {
            return new SmoothStepModifier(InsetWidth, GradientLength, MinMod, MaxMod);
        }
    }
}
