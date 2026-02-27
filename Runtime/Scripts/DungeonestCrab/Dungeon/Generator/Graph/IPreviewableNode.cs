using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [System.Serializable] public struct DungeonConnection { }
    [System.Serializable] public struct SourceConnection { }
    [System.Serializable] public struct NoiseConnection { }
    [System.Serializable] public struct BoundsConnection { }
    [System.Serializable] public struct MatcherConnection { }
    [System.Serializable] public struct EntitySourceConnection { }
    [System.Serializable] public struct ConnectorConnection { }
    [System.Serializable] public struct PathfinderConnection { }
    [System.Serializable] public struct CarverConnection { }
    [System.Serializable] public struct NoiseModifierConnection { }
    [System.Serializable] public struct CostProviderConnection { }
    [System.Serializable] public struct ActionConnection { }
    [System.Serializable] public struct NoiseBandConnection { }


    public interface IPreviewableNode {
        Texture2D GetPreviewTexture();
        void UpdatePreview();
    }
}