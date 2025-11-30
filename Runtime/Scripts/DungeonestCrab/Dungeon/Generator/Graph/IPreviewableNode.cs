using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [System.Serializable] public struct DungeonConnection { }
    [System.Serializable] public struct SourceConnection { }
    [System.Serializable] public struct NoiseConnection { }
    [System.Serializable] public struct BoundsConnection { }
    [System.Serializable] public struct MatcherConnection { }
    [System.Serializable] public struct EntitySourceConnection { }

    public interface IPreviewableNode {
        Texture2D GetPreviewTexture();
        void UpdatePreview();
    }
}