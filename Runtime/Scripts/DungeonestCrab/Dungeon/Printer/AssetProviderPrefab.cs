using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Provider - GameObject")]
    [InlineEditor]
    public class AssetProviderPrefab : IAssetProvider<GameObject> {}
}