using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "DungeonestCrab/Spec/Bounds - Multi Options")]
    public class BoundsComboSO : BoundsSO {
        public enum BoundsType {
            Full = 0,
            InsetRect,
            InsetPercent,
            InsetFixed,
            CenteredPercent,
            CenteredFixed,
            AbsoluteRect
        }

        [SerializeField] BoundsType Type;

        [ShowIf("Type", BoundsType.InsetFixed)]
        [SerializeField] InsetBounds InsetFixed;

        [ShowIf("Type", BoundsType.InsetRect)]
        [SerializeField] FixedBounds InsetRect;

        [ShowIf("Type", BoundsType.InsetPercent)]
        [SerializeField] InsetPercentBounds InsetPercent;

        [ShowIf("Type", BoundsType.CenteredPercent)]
        [SerializeField] CenteredBounds CenteredPercent;

        [ShowIf("Type", BoundsType.CenteredFixed)]
        [SerializeField] FixedCenterBounds CenteredFixed;

        [ShowIf("Type", BoundsType.AbsoluteRect)]
        [SerializeField] AbsoluteBounds AbsoluteRect;

        public override Bounds ToBounds() {
            switch (Type) {
                case BoundsType.Full:
                    return new FullBounds();
                case BoundsType.InsetRect:
                    return InsetRect;
                case BoundsType.InsetPercent:
                    return InsetPercent;
                case BoundsType.InsetFixed:
                    return InsetFixed;
                case BoundsType.CenteredPercent:
                    return CenteredPercent;
                case BoundsType.CenteredFixed:
                    return CenteredFixed;
                case BoundsType.AbsoluteRect:
                    return AbsoluteRect;
            }

            Debug.LogWarning($"Unhandled bounds type {Type}. Falling back to full.");
            return new FullBounds();
        }
    }
}