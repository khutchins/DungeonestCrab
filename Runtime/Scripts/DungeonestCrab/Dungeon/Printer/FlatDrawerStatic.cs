using Pomerandomian;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {

    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Flat - Static")]
    public class FlatDrawerStatic : IFlatDrawer {
        [HideIf("@this.PrefabProvider != null")]
        [SerializeField] GameObject Prefab;
        [HideIf("@this.Prefab != null")]
        [SerializeField] AssetProviderPrefab PrefabProvider;

        public override GameObject DrawFlat(Transform parent, IRandom random, TileSpec tileSpec) {
            GameObject prefab;
            if (Prefab != null) {
                prefab = Prefab;
            } else if (PrefabProvider == null || (prefab = PrefabProvider.GetAsset(random)) == null) {
                Debug.LogWarning($"No floor prefabs specified for {tileSpec.Terrain.ID}.");
                return new GameObject();
            }
            return Instantiate(prefab, parent);
        }

        private void OnValidate() {
            if (Prefab != null) {
                PrefabProvider = null;
            }
        }
    }
}