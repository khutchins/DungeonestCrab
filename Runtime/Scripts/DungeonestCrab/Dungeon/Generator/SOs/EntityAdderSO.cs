using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Entity Adder - Multi Options")]
    public class EntityAdderSO : AltererSO {
        public enum EntityAdderType {
            AddBorderEntity,
        }

        [SerializeField] EntityAdderType Type;


        public override IAlterer ToAlterer() {
            switch (Type) {
            }
            throw new System.NotImplementedException();
        }
    }
}