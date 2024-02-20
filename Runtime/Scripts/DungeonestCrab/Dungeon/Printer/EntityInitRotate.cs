using Pomerandomian;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public class EntityInitRotate : IEntityInit {
        [Tooltip("Each axis will be rotated by [0, MaxAngle.?] degrees.")]
        [SerializeField] Vector3 MaxAngles = Vector3.zero;
        [Tooltip("If this is true, rotation will be at multiples of a specific angle, not continuous.")]
        [SerializeField] bool RotateInIncrements;
        [ShowIf("RotateInIncrements")]
        [Tooltip("Increments to rotate at. If zero, will be continuous.")]
        [SerializeField] Vector3 AngleIncrements = Vector3.zero;

        public override void DoInit(Entity entity, Vector2Int pt, IRandom random) {
            this.transform.localEulerAngles = new Vector3(
                GetAngle(random, MaxAngles.x, RotateInIncrements, AngleIncrements.x),
                GetAngle(random, MaxAngles.y, RotateInIncrements, AngleIncrements.y),
                GetAngle(random, MaxAngles.z, RotateInIncrements, AngleIncrements.z)
            );
        }

        private float GetAngle(IRandom random, float maxAngle, bool shouldUseIncrements, float increment) {
            if (!shouldUseIncrements || increment <= 0) return random.Next(0, maxAngle);
            return random.Next(Mathf.FloorToInt(maxAngle / increment)) * increment;
        }
    }
}