using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public abstract class IEntityInit : MonoBehaviour {
        public abstract void DoInit(GameObject go, Entity entity, IRandom random);
    }
}