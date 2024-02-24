using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    public class DungeonEntrance : MonoBehaviour, DungeonGrid.GridObject {
        public string Name;
        public float Angle;
        public bool Default;

        void Awake() {
            DungeonGrid.INSTANCE.RegisterEntrance(this);
        }

        public DungeonGrid.GridItemInfo GridItemInfo() {
            return DungeonGrid.INSTANCE.InfoForLocation(transform.position);
        }
    }
}