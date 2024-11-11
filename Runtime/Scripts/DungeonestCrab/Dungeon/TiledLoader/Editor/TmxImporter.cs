using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AssetImporters;
#endif
using System.IO;

namespace DungeonestCrab.Dungeon.TiledLoader {
#if UNITY_EDITOR
    [ScriptedImporter(1, "tmx")]
    public class TmxImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            TextAsset subAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("text", subAsset);
            ctx.SetMainObject(subAsset);
        }
    }
#endif
}