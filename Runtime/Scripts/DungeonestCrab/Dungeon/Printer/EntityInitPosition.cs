using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public class EntityInitPosition : IEntityInit {
        [SerializeField] Vector3 MaxRange = Vector3.zero;

        public override void DoInit(Entity entity, Vector2Int pt, IRandom random) {
            MaxRange.x = Mathf.Abs(MaxRange.x);
            MaxRange.y = Mathf.Abs(MaxRange.y);
            MaxRange.z = Mathf.Abs(MaxRange.z);
            this.transform.localPosition = this.transform.localPosition +
                new Vector3(
                    MaxRange.x <= 0 ? random.Next(-MaxRange.x, MaxRange.x) : 0,
                    MaxRange.y <= 0 ? random.Next(-MaxRange.y, MaxRange.y) : 0,
                    MaxRange.z <= 0 ? random.Next(-MaxRange.z, MaxRange.z) : 0);
        }
    }
}