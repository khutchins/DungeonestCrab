using UnityEngine;
using Sirenix.OdinInspector;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Flat - Multi")]
    public class FlatDrawerMulti : IFlatDrawer {

        [SerializeField]
        [InlineEditor]
        private IFlatDrawer[] Drawers;

        public override GameObject DrawFlat(FlatInfo info) {
            GameObject container = new GameObject($"MultiFlat {info.tileSpec.Coords}");

            if (Drawers != null) {
                foreach (var drawer in Drawers) {
                    if (drawer != null) {
                        GameObject layer = drawer.DrawFlat(info);
                        if (layer != null) {
                            layer.transform.SetParent(container.transform, false);
                        }
                    }
                }
            }

            return container;
        }
    }
}