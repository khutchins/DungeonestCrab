using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Floor - Quad - Directional")]
    public class FlatDrawerQuadSimpleDirectional : FlatDrawerMesh {
        [InlineEditor][SerializeField] TextureView FloorNESW;
        [InlineEditor][SerializeField] TextureView FloorNES;
        [InlineEditor][SerializeField] TextureView FloorNS;
        [InlineEditor][SerializeField] TextureView FloorNE;
        [InlineEditor][SerializeField] TextureView FloorN;

        protected Dictionary<int, Vector2[]> _directionalUVs;
        protected Dictionary<int, Material> _directionalMats;

        protected override void RecomputeCache() {
            // N E S W
            _directionalUVs = new Dictionary<int, Vector2[]>() {
                { 0b0000, FloorNESW.UV },
                { 0b1000, FloorN.TurnedUV(3) },
                { 0b0100, FloorN.TurnedUV(2) },
                { 0b0010, FloorN.TurnedUV(1) },
                { 0b0001, FloorN.TurnedUV(0) },
                { 0b1100, FloorNE.TurnedUV(3) },
                { 0b0110, FloorNE.TurnedUV(2) },
                { 0b0011, FloorNE.TurnedUV(1) },
                { 0b1001, FloorNE.TurnedUV(0) },
                { 0b1010, FloorNS.TurnedUV(1) },
                { 0b0101, FloorNS.TurnedUV(0) },
                { 0b1110, FloorNES.TurnedUV(3) },
                { 0b0111, FloorNES.TurnedUV(2) },
                { 0b1011, FloorNES.TurnedUV(1) },
                { 0b1101, FloorNES.TurnedUV(0) },
                { 0b1111, FloorNESW.UV },
            };
            _directionalMats = new Dictionary<int, Material>() {
                { 0b0000, FloorNESW.Material },
                { 0b1000, FloorN.Material },
                { 0b0100, FloorN.Material },
                { 0b0010, FloorN.Material },
                { 0b0001, FloorN.Material },
                { 0b1100, FloorNE.Material },
                { 0b0110, FloorNE.Material },
                { 0b0011, FloorNE.Material },
                { 0b1001, FloorNE.Material },
                { 0b1010, FloorNS.Material },
                { 0b0101, FloorNS.Material },
                { 0b1110, FloorNES.Material },
                { 0b0111, FloorNES.Material },
                { 0b1011, FloorNES.Material },
                { 0b1101, FloorNES.Material },
                { 0b1111, FloorNESW.Material },
            };
        }

        public override bool DrawAtCenter { get => false; }

        public const int DNRTH = 0b1000;
        public const int DEAST = 0b0100;
        public const int DSUTH = 0b0010;
        public const int DWEST = 0b0001;

        public override void Draw(FlatInfo info, Mesher mesher) {
            int neighbors = 0;
            if (info.tileSpec.Coords.x == 3 && info.tileSpec.Coords.y == 4) {
                int i = 1;
                i++;
            }
            if (info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.N)) neighbors |= DNRTH;
            if (info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.E)) neighbors |= DEAST;
            if (info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.S)) neighbors |= DSUTH;
            if (info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.W)) neighbors |= DWEST;

            Material mat = _directionalMats[neighbors];
            var uvs = _directionalUVs[neighbors];
            mesher.GenerateRect(mat, 
                mesher.AddVert(mat, new Vector3(0, 0, 0), uvs[0]),
                mesher.AddVert(mat, new Vector3(info.tileSize.x, 0, 0), uvs[3]),
                mesher.AddVert(mat, new Vector3(0, 0, info.tileSize.z), uvs[1]),
                mesher.AddVert(mat, new Vector3(info.tileSize.x, 0, info.tileSize.z), uvs[2])
            );
        }

        
    }
}