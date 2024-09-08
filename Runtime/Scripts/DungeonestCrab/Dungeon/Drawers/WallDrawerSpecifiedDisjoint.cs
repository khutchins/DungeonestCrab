using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Specified Disjoint")]
    public class WallDrawerSpecifiedDisjoint : WallDrawerSpecifiedBase {
        [SerializeField] WallSpecDisjoint[] WallConfiguration;
        private WallDrawerSpecifiedBase.WallSpec[] _cache = null;

        private void OnValidate() {
            _cache = null;
        }

        WallSpec[] EnsureCache() {
            if (_cache != null) return _cache;

            List<WallSpecDisjoint> wallSpecs = WallConfiguration.ToList();

            List<WallSpec> generatedWallSpecs = new List<WallSpec>();

            for (int i = 0; i < wallSpecs.Count; i++) {
                WallSpecDisjoint curr = wallSpecs[i];
                if (curr.endsUVY != curr.startsUVY) {
                    generatedWallSpecs.Add(curr.ToWallSpecEnd());
                }
                generatedWallSpecs.Add(curr.ToWallSpecStart());
            }
            _cache = generatedWallSpecs.ToArray();
            return _cache;
        }

        protected override WallSpec[] GetWallConfiguration() {
            return EnsureCache();
        }

        [System.Serializable]
        public class WallSpecDisjoint {
            public float percentY = 0;
            public float outset = 0;
            public float endsUVY = 0;
            public float startsUVY = 0;

            public WallSpecDisjoint(float percentY, float outset, float endsUVY, float startsUVY) {
                this.percentY = percentY;
                this.outset = outset;
                this.endsUVY = endsUVY;
                this.startsUVY = startsUVY;
            }

            public WallSpec ToWallSpecStart() {
                return new WallSpec(percentY, outset, startsUVY);
            }

            public WallSpec ToWallSpecEnd() {
                return new WallSpec(percentY, outset, endsUVY);
            }
        }
    }
}