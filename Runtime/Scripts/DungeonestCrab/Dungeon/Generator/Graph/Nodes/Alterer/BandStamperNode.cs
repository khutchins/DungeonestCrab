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
            var bands = bandHolders != null ? 
                new List<BandStamper.BandEntry>(bandHolders) : 
                new List<BandStamper.BandEntry>();

            var stamper = new BandStamper(noise, modifier, bands, bounds);
            return stamper.Modify(dungeon, random);
        }
    }
}
