using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Bounds - Inset Percent")]
    public class BoundsInsetPercentSO : BoundsSO {
        [HorizontalGroup(Width = 0.5f)]
        public float L = 1;
        [HorizontalGroup(Width = 0.5f)]
        public float R = 1;
        [HorizontalGroup("row2", Width = 0.5f)]
        public float T = 1;
        [HorizontalGroup("row2", Width = 0.5f)]
        public float B = 1;


        public override Bounds ToBounds() {
            return new InsetPercentBounds(L, R, T, B);
        }
    }
}