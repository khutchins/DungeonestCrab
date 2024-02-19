using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Spec")]
    [InlineEditor]
    public class DungeonSpecSO : ScriptableObject {
        [SerializeField] Trait Trait = Trait.None;
        [SerializeField] float FogDensity = 0.1f;
        [SerializeField] Color FogColor = Color.black;
        [Tooltip("Size of the world in tiles.")]
        [SerializeField] Vector2Int Size = new Vector2Int(40, 40);
        [SerializeField] AltererSO[] Alterers;

        public DungeonSpec ToDungeonSpec() {
            return ToDungeonSpec(ConvertedAlterers());
        }

        private IEnumerable<IAlterer> ConvertedAlterers() {
            return Alterers.Select(x => x.ToAlterer());
        }

        public DungeonSpec ToDungeonSpec(IEnumerable<IAlterer> startAlterers = null, IEnumerable<IAlterer> endAlterers = null) {
            if (startAlterers == null) startAlterers = Enumerable.Empty<IAlterer>();
            if (endAlterers == null) endAlterers = Enumerable.Empty<IAlterer>();
            return ToDungeonSpec(startAlterers.Concat(ConvertedAlterers()).Concat(endAlterers));
        }

        private DungeonSpec ToDungeonSpec(IEnumerable<IAlterer> allAlterers) {
            return new DungeonSpec(Trait, Size, allAlterers.ToArray(), FogDensity, FogColor);
        }
    }
}