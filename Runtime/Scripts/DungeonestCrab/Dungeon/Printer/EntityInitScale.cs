using DungeonestCrab.Dungeon.Printer;
using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public class EntityInitScale : IEntityInit {
        [SerializeField] float MinScale = 1;
        [SerializeField] float MaxScale = 1;

        public override void DoInit(GameObject go, Entity entity, IRandom random) {
            float scale = random.Next(MinScale, MaxScale);
            this.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}