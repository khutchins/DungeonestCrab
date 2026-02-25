using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Noise/Modifiers/Modifier Mix")]
    public class NoiseModifierMixNode : NoiseModifierProviderNode {
        public enum MixMode {
            Max,
            Min,
            Add,
            Multiply
        }

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseModifierConnection A;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseModifierConnection B;
        public MixMode Mode = MixMode.Max;

        public override INoiseModifier CreateModifier() {
            var modA = GetInputValue<INoiseModifier>("A", null);
            var modB = GetInputValue<INoiseModifier>("B", null);

            return new MixedModifier(modA, modB, Mode);
        }

        private class MixedModifier : INoiseModifier {
            private readonly INoiseModifier _a;
            private readonly INoiseModifier _b;
            private readonly MixMode _mode;

            public MixedModifier(INoiseModifier a, INoiseModifier b, MixMode mode) {
                _a = a;
                _b = b;
                _mode = mode;
            }

            public float Modifier(AppliedBounds bounds, int x, int y) {
                float valA = _a?.Modifier(bounds, x, y) ?? 0;
                float valB = _b?.Modifier(bounds, x, y) ?? 0;

                switch (_mode) {
                    case MixMode.Max: return Mathf.Max(valA, valB);
                    case MixMode.Min: return Mathf.Min(valA, valB);
                    case MixMode.Add: return valA + valB;
                    case MixMode.Multiply: return valA * valB;
                    default: return valA;
                }
            }
        }
    }
}
