using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Bounds - Full")]
    public class BoundsFullSO : BoundsSO {


        public override Bounds ToBounds() {
            return new FullBounds();
        }
    }
}