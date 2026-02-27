using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;
using System.Collections.Generic;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Stamp (Bands)")]
    public class BandStamperNode : DungeonPassNode {

        [Header("Inputs")]
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public NoiseConnection Noise;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public NoiseModifierConnection NoiseModifier;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public BoundsConnection Bounds;

        [Header("Bands")]
        [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)] public NoiseBandConnection Bands;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            INoiseSource noise = GetInputValue<INoiseSource>("Noise", null);
            if (noise == null) return true;

            INoiseModifier modifier = GetInputValue<INoiseModifier>("NoiseModifier", null);
            Bounds bounds = GetInputValue<Bounds>("Bounds", null);

            var bandHolders = GetInputValues<BandStamper.BandEntry>("Bands", null);
            var bands = new List<BandStamper.BandEntry>(bandHolders);

            var stamper = new BandStamper(noise, modifier, bands, bounds);
            return stamper.Modify(dungeon, random);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Noise Band")]
    public class NoiseBandNode : Node {
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public ActionConnection Action;
        [Output(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public NoiseBandConnection Output;

        [Range(0, 1)] public float MinThreshold = 0f;
        [Range(0, 1)] public float MaxThreshold = 1f;

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") {
                return new BandStamper.BandEntry {
                    Min = MinThreshold,
                    Max = MaxThreshold,
                    Action = GetInputValue<ITileAction>("Action", null)
                };
            }
            return null;
        }
    }
}
