using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

namespace DungeonestCrab.Dungeon.TiledLoader {
    [ScriptedImporter(1, "tmx")]
    public class TmxImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            TextAsset subAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("text", subAsset);
            ctx.SetMainObject(subAsset);
        }
    }
}