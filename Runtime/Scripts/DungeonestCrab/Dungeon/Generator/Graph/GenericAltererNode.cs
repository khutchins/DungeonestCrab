using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Legacy/Alterer SO Wrapper")]
    public class GenericAltererNode : DungeonPassNode {
        public AltererSO AltererScriptableObject;

        protected override void ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            if (AltererScriptableObject != null) {
                IAlterer alterer = AltererScriptableObject.ToAlterer();
                alterer.Modify(dungeon, random);
            }
        }
    }
}