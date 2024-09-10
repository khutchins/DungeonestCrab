using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Floor - Quad")]
    public class FlatDrawerQuad : FlatDrawerMesh {
        [InlineEditor][SerializeField] TextureView FloorTexture;

        private Vector2[] _uv;

        private void OnEnable() {
            RecomputeGarbage();
        }

        private void OnValidate() {
            RecomputeGarbage();
        }

        void RecomputeGarbage() {
            _uv = FloorTexture.UV;
        }

        public override bool DrawAtCenter { get => false; }

        public override void Draw(FlatInfo info, Mesher mesher) {
            mesher.GenerateRect(
                mesher.AddVert(new Vector3(0, 0, 0), _uv[0]),
                mesher.AddVert(new Vector3(info.tileSize.x, 0, 0), _uv[3]),
                mesher.AddVert(new Vector3(0, 0, info.tileSize.z), _uv[1]),
                mesher.AddVert(new Vector3(info.tileSize.x, 0, info.tileSize.z), _uv[2])
            );
        }
    }
}