using System.Collections.Generic;
using System.IO;
using System.Linq;
using DungeonestCrab.Dungeon.Printer;
using UnityEditor;
using UnityEngine;

namespace DungeonestCrab.Editor {
    public static class CreateDrawerFromMaterialMenu {
        const string MenuRoot = "Assets/Create/DungeonestCrab/From Material/";

        [MenuItem(MenuRoot + "TextureView", false, 100)]
        static void CreateTextureView() {
            Object last = null;
            foreach (Material mat in SelectedMaterials()) {
                var tv = MakeTextureViewForWholeMaterial(mat);
                string path = AssetDatabase.GenerateUniqueAssetPath(
                    $"{MaterialFolder(mat)}/{mat.name}_TextureView.asset");
                AssetDatabase.CreateAsset(tv, path);
                last = tv;
            }
            AssetDatabase.SaveAssets();
            if (last != null) Selection.activeObject = last;
        }

        [MenuItem(MenuRoot + "TextureView", true)]
        static bool CreateTextureViewValidate() => HasMaterialSelected();

        [MenuItem(MenuRoot + "Floor - Quad", false, 101)]
        static void CreateFloorQuad() {
            CreateDrawersForSelection<FlatDrawerQuad>("FloorTexture", "Floor");
        }

        [MenuItem(MenuRoot + "Floor - Quad", true)]
        static bool CreateFloorQuadValidate() => HasMaterialSelected();

        [MenuItem(MenuRoot + "Wall - Mesh Tiled", false, 102)]
        static void CreateWallMeshTiled() {
            CreateDrawersForSelection<WallDrawerMeshTiled>("Texture", "Wall");
        }

        [MenuItem(MenuRoot + "Wall - Mesh Tiled", true)]
        static bool CreateWallMeshTiledValidate() => HasMaterialSelected();

        static void CreateDrawersForSelection<T>(string textureViewFieldName, string assetPrefix)
                where T : ScriptableObject {
            Object last = null;
            foreach (Material mat in SelectedMaterials()) {
                var drawer = ScriptableObject.CreateInstance<T>();
                string path = AssetDatabase.GenerateUniqueAssetPath(
                    $"{MaterialFolder(mat)}/{assetPrefix}_{mat.name}.asset");
                AssetDatabase.CreateAsset(drawer, path);

                var tv = MakeTextureViewForWholeMaterial(mat);
                tv.name = $"{mat.name} TextureView";
                AssetDatabase.AddObjectToAsset(tv, drawer);

                AssignProperty(drawer, textureViewFieldName, tv);

                AssetDatabase.ImportAsset(path);
                last = drawer;
            }
            AssetDatabase.SaveAssets();
            if (last != null) Selection.activeObject = last;
        }

        static TextureView MakeTextureViewForWholeMaterial(Material mat) {
            var tv = ScriptableObject.CreateInstance<TextureView>();
            var so = new SerializedObject(tv);
            so.FindProperty("_material").objectReferenceValue = mat;
            so.FindProperty("Tiles").vector2IntValue = new Vector2Int(1, 1);
            so.ApplyModifiedPropertiesWithoutUndo();
            return tv;
        }

        static void AssignProperty(Object target, string propName, Object value) {
            var so = new SerializedObject(target);
            so.FindProperty(propName).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static string MaterialFolder(Material mat) {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(mat)).Replace('\\', '/');
        }

        static IEnumerable<Material> SelectedMaterials() {
            return Selection.GetFiltered<Material>(SelectionMode.Assets);
        }

        static bool HasMaterialSelected() {
            return SelectedMaterials().Any();
        }
    }
}
