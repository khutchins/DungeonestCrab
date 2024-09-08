using DungeonestCrab.Dungeon.Printer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public abstract class FlatDrawerMesh : IFlatDrawer {
        [SerializeField] public Material Material;

        public override GameObject DrawFlat(FlatInfo info) {
            GameObject gameObject = new GameObject($"Flat {info.tileSpec.Coords}");
            GameObject drawObject = gameObject;
            if (!DrawAtCenter) {
                drawObject = new GameObject("inner");
                drawObject.transform.SetParent(gameObject.transform, false);
                drawObject.transform.position = new Vector3(-info.tileSize.x / 2f, 0, -info.tileSize.z / 2f);
            }
            Mesher mesher = new Mesher(drawObject, Material, GenerateCollider);
            Draw(info, mesher);
            mesher.Finish();
            return gameObject;
        }

        public virtual bool GenerateCollider { get => false; }

        public virtual bool DrawAtCenter { get => true; }

        public abstract void Draw(FlatInfo info, Mesher mesher);
    }
}