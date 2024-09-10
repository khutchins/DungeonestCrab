using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public interface ITextureView {
        Material Material { get; }
        Vector2[] UV { get; }
        Vector2[] TurnedUV(int turnCount);
        Vector2 ConvertToLocalUVSpace(Vector2 uv);
    }
    
    public static class TextureViewHelpers {
        public static Vector2[] TurnUV(Vector2[] uv, int count) {
            count %= 4;
            if (count == 0) return uv;
            Vector2[] turned = new Vector2[4];
            for (int i = 0; i < 4; i++) {
                int mi = (i + count) % 4;
                turned[i] = uv[mi];
            }
            return turned;
        }
    }
}