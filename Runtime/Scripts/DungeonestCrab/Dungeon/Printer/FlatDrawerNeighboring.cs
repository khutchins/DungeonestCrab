using Pomerandomian;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Flat - Neighbor-Aware")]
    public class FlatDrawerNeighboring : IFlatDrawer {
        [Header("Attributes")]
        [SerializeField] bool RequireSameTile = true;
        [SerializeField] bool RequireSameTerrain = false;
        [Header("Drawers")]
        [ValidateInput("ValidateInput", "This drawer must be provided!", InfoMessageType.Warning)]
        [SerializeField] IFlatDrawer DrawerNESW;
        [SerializeField] IFlatDrawer DrawerNES;
        [SerializeField] IFlatDrawer DrawerNE;
        [SerializeField] IFlatDrawer DrawerNS;
        [SerializeField] IFlatDrawer DrawerN;
        [SerializeField] IFlatDrawer DrawerNone;

        // These are populated based on the provided drawers above. None of these
        // will ever be null (unless DrawerNESW is not provided, which will fail
        // drawing anyway).
        [SerializeField] [HideInInspector] IFlatDrawer DrawerNESComputed;
        [SerializeField] [HideInInspector] IFlatDrawer DrawerNEComputed;
        [SerializeField] [HideInInspector] IFlatDrawer DrawerNSComputed;
        [SerializeField] [HideInInspector] IFlatDrawer DrawerNComputed;
        [SerializeField] [HideInInspector] IFlatDrawer DrawerNoneComputed;

        private bool GetAdjacency(TileSpec tile, TileSpec.Adjacency adjacency) {
            return true &&
                (RequireSameTile ? tile.AreTileTypesTheSameInDirections(adjacency) : true) &&
                (RequireSameTerrain ? tile.AreTerrainsTheSameInDirections(adjacency) : true);
        }

        public override GameObject DrawFlat(Transform parent, IRandom random, TileSpec tileSpec) {
            if (DrawerNESW == null) {
                Debug.LogWarning($"DrawerNESW must be set on flat drawer for terrain {tileSpec.Terrain.ID}");
            }
            bool n = GetAdjacency(tileSpec, TileSpec.Adjacency.N);
            bool e = GetAdjacency(tileSpec, TileSpec.Adjacency.E);
            bool s = GetAdjacency(tileSpec, TileSpec.Adjacency.S);
            bool w = GetAdjacency(tileSpec, TileSpec.Adjacency.W);

            IFlatDrawer drawer = DrawerNESW;
            float rotation = 0;
            if (n && e && s && w) {
                // Intentionally left blank.
            } else if (n && e && s) {
                drawer = DrawerNES;
            } else if (e && s && w) {
                drawer = DrawerNESComputed;
                rotation = 90;
            } else if (s && w && n) {
                drawer = DrawerNESComputed;
                rotation = 180;
            } else if (w && n && e) {
                drawer = DrawerNESComputed;
                rotation = 270;
            } else if (n && s) {
                drawer = DrawerNSComputed;
            } else if (e && w) {
                drawer = DrawerNSComputed;
                rotation = 90;
            } else if (n && e) {
                drawer = DrawerNEComputed;
            } else if (e && s) {
                drawer = DrawerNEComputed;
                rotation = 90;
            } else if (s && w) {
                drawer = DrawerNEComputed;
                rotation = 180;
            } else if (w && n) {
                drawer = DrawerNEComputed;
                rotation = 270;
            } else if (n) {
                drawer = DrawerNComputed;
            } else if (e) {
                drawer = DrawerNComputed;
                rotation = 90;
            } else if (s) {
                drawer = DrawerNComputed;
                rotation = 180;
            } else if (w) {
                drawer = DrawerNComputed;
                rotation = 270;
            } else {
                drawer = DrawerNoneComputed;
            }
            GameObject go = drawer.DrawFlat(parent, random, tileSpec);
            go.transform.localEulerAngles = new Vector3(0, rotation, 0);
            return go;
        }

        private void OnValidate() {
            DrawerNESComputed = DrawerNES != null ? DrawerNES : DrawerNESW;
            DrawerNSComputed = DrawerNS != null ? DrawerNS : DrawerNESW;
            DrawerNEComputed = DrawerNE != null ? DrawerNE : DrawerNESComputed;
            DrawerNComputed = DrawerN != null ? DrawerN : DrawerNSComputed;
            DrawerNoneComputed = DrawerNone != null ? DrawerNone : DrawerNComputed;
        }

        private bool ValidateInput(IFlatDrawer drawer) {
            return drawer != null;
        }
    }
}