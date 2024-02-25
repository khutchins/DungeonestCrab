using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "DungeonestCrab/Spec/Source - River")]
    [InlineEditor]
    public class SourceRiverSO : SourceSO {
        [Range(0f, 10f)]
        public float MinWidth = 0.5f;
        [Range(0f, 10f)]
        public float MaxWidth = 1.5f;
        public SourceRiver.Sides StartSides = SourceRiver.Sides.Top | SourceRiver.Sides.Left | SourceRiver.Sides.Right | SourceRiver.Sides.Bottom;
        public SourceRiver.Sides EndSides = SourceRiver.Sides.Top | SourceRiver.Sides.Left | SourceRiver.Sides.Right | SourceRiver.Sides.Bottom;

        public override ISource ToSource() {
            return new SourceRiver(TileToSet, MinWidth, MaxWidth, StartSides, EndSides);
        }
    }
}