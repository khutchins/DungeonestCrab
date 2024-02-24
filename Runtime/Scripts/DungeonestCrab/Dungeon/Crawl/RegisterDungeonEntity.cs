using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    public class RegisterDungeonEntity : MonoBehaviour {
        [SerializeField] DungeonEntityReference Reference;

        private void Awake() {
            Reference.Value = GetComponent<DungeonEntity>();
        }
    }
}