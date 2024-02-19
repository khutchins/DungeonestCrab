using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	[CreateAssetMenu(menuName = "Dungeon/Spec/Bounds - Inset Fixed")]
    public class BoundsInsetSO : BoundsSO {
        [HorizontalGroup(Width = 0.5f)]
        public int L = 1;
        [HorizontalGroup(Width = 0.5f)]
        public int R = 1;
        [HorizontalGroup("row2", Width = 0.5f)]
        public int T = 1;
        [HorizontalGroup("row2", Width = 0.5f)]
        public int B = 1;


        public override Bounds ToBounds() {
            return new InsetBounds(L, R, T, B);
        }
    }
}