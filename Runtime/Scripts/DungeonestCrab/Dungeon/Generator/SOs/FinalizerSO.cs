using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Finalizer")]
    [InlineEditor]
    public class FinalizerSO : AltererSO {
        public TerrainSO Terrain;

        public override IAlterer ToAlterer() {
            return new Finalizer(Terrain);
        }
    }
}