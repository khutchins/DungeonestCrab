using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Legacy/Alterer SO Wrapper")]
    public class GenericAltererNode : DungeonPassNode {
        public AltererSO AltererScriptableObject;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            if (AltererScriptableObject != null) {
                IAlterer alterer = AltererScriptableObject.ToAlterer();
                return alterer.Modify(dungeon, random);
            }
            return false;
        }
    }
}