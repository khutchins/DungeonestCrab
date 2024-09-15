using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Floor - Quad - Autotile")]
    public class FlatDrawerAutotile : FlatDrawerMesh {
        [Tooltip("Texture view in RPGMaker autotile format.")]
        [InlineEditor][SerializeField] TextureView Autotile;

        protected List<List<Vector2[]>> _uvs = new List<List<Vector2[]>>();


        private static readonly Vector3[,] UVS = new Vector3[,] { {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0.5f),
                new Vector3(0, 0, 1f),
            }, {
                new Vector3(0.5f, 0, 0),
                new Vector3(0.5f, 0, 0.5f),
                new Vector3(0.5f, 0, 1f),
            }, {
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 0.5f),
                new Vector3(1, 0, 1f),
            }
        };

        private const int NW = 0;
        private const int NE = 1;
        private const int SW = 2;
        private const int SE = 3;

        private const int INNER = 0;
        private const int OUTER = 1;
        private const int X_SIDE = 2;
        private const int Y_SIDE = 3;
        private const int ALL = 4;

        protected override void RecomputeCache() {
            Vector2[] UVsForXY(int x, int y) {
                return Autotile.SubUV(x, y, 1, 1, 4, 6, 3);
            }

            // See https://www.rpgmakerweb.com/blog/classic-tutorial-how-autotiles-work.
            _uvs = new List<List<Vector2[]>> {
                new List<Vector2[]> { // NW
                    UVsForXY(2, 0), // Inner
                    UVsForXY(0, 2), // Outer
                    UVsForXY(0, 4), // X-Side
                    UVsForXY(2, 2), // Y-Side
                    UVsForXY(2, 4)  // All
                },
                new List<Vector2[]> { // NE
                    UVsForXY(3, 0),
                    UVsForXY(3, 2),
                    UVsForXY(3, 4),
                    UVsForXY(1, 2),
                    UVsForXY(1, 4)
                },
                new List<Vector2[]> { // SW
                    UVsForXY(2, 1),
                    UVsForXY(0, 5),
                    UVsForXY(0, 3),
                    UVsForXY(2, 5),
                    UVsForXY(2, 3)
                },
                new List<Vector2[]> { // SE
                    UVsForXY(3, 1),
                    UVsForXY(3, 5),
                    UVsForXY(3, 3),
                    UVsForXY(1, 5),
                    UVsForXY(1, 3)
                }
            };
        }

        public override bool DrawAtCenter { get => false; }

        public override void Draw(FlatInfo info, Mesher mesher) {
            int Idx(TileSpec.Adjacency Ver, TileSpec.Adjacency Hor, TileSpec.Adjacency Both) {
                if (info.tileSpec.AreTileTypesTheSameInDirections(Ver)) {
                    if (info.tileSpec.AreTileTypesTheSameInDirections(Hor)) {
                        if (info.tileSpec.AreTileTypesTheSameInDirections(Both)) {
                            return ALL;
                        }
                        return INNER;
                    } else {
                        // Ver same, Hor different
                        return X_SIDE;
                    }
                } else {
                    // N different
                    if (info.tileSpec.AreTileTypesTheSameInDirections(Hor)) {
                        // Ver different, Hor same
                        return Y_SIDE;
                    }
                    // Ver different, Hor different
                    return OUTER;
                }
            }

            Material mat = Autotile.Material;
            {
                // NW
                int idx = Idx(TileSpec.Adjacency.N, TileSpec.Adjacency.W, TileSpec.Adjacency.NW);
                var uvs = _uvs[NW][idx];
                mesher.GenerateRect(mat,
                    mesher.AddVert(mat, Vector3.Scale(UVS[0, 1], info.tileSize), uvs[0]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 1], info.tileSize), uvs[3]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[0, 2], info.tileSize), uvs[1]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 2], info.tileSize), uvs[2])
                );
            }
            {
                // NE
                int idx = Idx(TileSpec.Adjacency.N, TileSpec.Adjacency.E, TileSpec.Adjacency.NE);
                var uvs = _uvs[NE][idx];
                mesher.GenerateRect(mat,
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 1], info.tileSize), uvs[0]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[2, 1], info.tileSize), uvs[3]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 2], info.tileSize), uvs[1]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[2, 2], info.tileSize), uvs[2])
                );
            }
            {
                // SW
                int idx = Idx(TileSpec.Adjacency.S, TileSpec.Adjacency.W, TileSpec.Adjacency.SW);
                var uvs = _uvs[SW][idx];
                mesher.GenerateRect(mat,
                    mesher.AddVert(mat, Vector3.Scale(UVS[0, 0], info.tileSize), uvs[0]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 0], info.tileSize), uvs[3]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[0, 1], info.tileSize), uvs[1]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 1], info.tileSize), uvs[2])
                );
            }
            {
                // SE
                int idx = Idx(TileSpec.Adjacency.S, TileSpec.Adjacency.E, TileSpec.Adjacency.SE);
                var uvs = _uvs[SE][idx];
                mesher.GenerateRect(mat,
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 0], info.tileSize), uvs[0]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[2, 0], info.tileSize), uvs[3]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[1, 1], info.tileSize), uvs[1]),
                    mesher.AddVert(mat, Vector3.Scale(UVS[2, 1], info.tileSize), uvs[2])
                );
            }
        }
    }
}