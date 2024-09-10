using System.Collections;
using System.Collections.Generic;
using System.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

[CreateAssetMenu(menuName = "DungeonestCrab/TextureView")]
public class TextureView : ScriptableObject, ITextureView {
    [SerializeField] Material _material;
    [Tooltip("How many tiles the texture has in each direction")]
    [SerializeField] Vector2Int Tiles = new Vector2Int(4,4);
    [Tooltip("The offset in tiles from the top left.")]
    [SerializeField] Vector2 Offset = new Vector2(0,0);
    [Tooltip("The size in tiles.")]
    [SerializeField] Vector2 Size = new Vector2(1,1);
    [Tooltip("How many times to rotate the texture (not visualized).")]
    [SerializeField] public int Turns = 0;
    public Material Material => _material;

    public Vector2[] UV => DirectionalUV(Offset.x, Offset.y, Size.x, Size.y, Tiles.x, Tiles.y, Turns);

    public Vector2[] TurnedUV(int turnCount) {
        return TurnUV(UV, turnCount);
    }

    public static Vector2[] DirectionalUV(float x, float y, float w, float h, int tx, int ty, int turns = 0) {
        float sx = x * 1f / tx;
        float ex = (x + w) * 1f / tx;
        float sy = 1 - (y * 1f / ty);
        float ey = 1 - ((y + h) * 1f / ty);

        Vector2[] unaltered = new Vector2[4] { new Vector2(sx, sy), new Vector2(ex, sy), new Vector2(ex, ey), new Vector2(sx, ey) };
        return TurnUV(unaltered, turns);
    }

    private static Vector2[] TurnUV(Vector2[] uv, int count) {
        count %= 4;
        if (count == 0) return uv;
        Vector2[] turned = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            int mi = (i + count) % 4;
            turned[i] = uv[mi];
        }
        return turned;
    }

    public Vector2Int TextureSize {
        get {
            var tex = Material.GetTexture("_MainTex");
            return new Vector2Int(Mathf.FloorToInt(tex.width / Tiles.x * Size.x), Mathf.FloorToInt(tex.height / Tiles.y * Size.y));
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextureView))]
public class TextureViewEditor : Editor {
    private Texture _texture;

    public override void OnInspectorGUI() {
        TextureView view = target as TextureView;

        // Draw default inspector
        DrawDefaultInspector();
        DrawPreview(view);
        DrawFullTexture(view);
    }

    private void DrawFullTexture(TextureView view) {
        if (view.Material == null) return;
        Texture2D mainTex = view.Material.GetTexture("_MainTex") as Texture2D;
        if (mainTex == null) return;

        EditorGUILayout.LabelField($"Base Texture ({mainTex.width}, {mainTex.height})", EditorStyles.boldLabel);
        var size = ScaledMinSize(new Vector2(128, 128), view.TextureSize);
        Rect texRect = GUILayoutUtility.GetRect(size.x, size.y);
        EditorGUI.DrawTextureTransparent(texRect, mainTex, ScaleMode.ScaleToFit, 0, 0);
    }

    private void DrawPreview(TextureView view) {

        if (view.Material == null) {
            EditorGUILayout.LabelField($"Texture Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("No material.");
            return;
        }
        Texture2D mainTex = view.Material.GetTexture("_MainTex") as Texture2D;
        if (mainTex == null) {
            EditorGUILayout.LabelField($"Texture Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("No _MainTex component. Should still work, but won't preview.");
            return;
        }

        if (_texture == null || GUI.changed) {
            _texture = CreateTexture(view);
        }
        EditorGUILayout.LabelField($"Texture Preview {view.TextureSize}", EditorStyles.boldLabel);
        var size = ScaledMinSize(new Vector2(128, 128), view.TextureSize);
        Rect texRect = GUILayoutUtility.GetRect(size.x, size.y);
        EditorGUI.DrawTextureTransparent(texRect, _texture, ScaleMode.ScaleToFit, 0, 0);
    }

    private Vector2 ScaledMinSize(Vector2 targetSize, Vector2 size) {
        int sx = Mathf.FloorToInt(targetSize.x / size.x);
        int sy = Mathf.FloorToInt(targetSize.y / size.y);
        int value = Mathf.Max(1, Mathf.Min(sx, sy));
        return new Vector2(size.x * value, size.y * value);
    }

    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise) {
        // The performance of this is non-optimal (especially how I'm generating multiple
        // intermediate textures, but this is just for the inspector, so I'll wait for it
        // to be a problem there.
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j) {
            for (int i = 0; i < w; ++i) {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w, originalTexture.format, false);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    private Texture2D CreateTexture(TextureView target) {
        if (target == null) return null;

        Texture bigTex = target.Material.GetTexture("_MainTex");
        if (bigTex is not Texture2D) {
            Debug.Log("Can't preview texture view as texture is not a Texture2D. It should still work though.");
            return null;
        }
        Vector2Int size = target.TextureSize;
        Texture2D newTex = new Texture2D((int)size.x, (int)size.y, (bigTex as Texture2D).format, false);

        Rect texCoords = new Rect(bigTex.width, bigTex.height, 0, 0);
        foreach (var vec in target.UV) {
            texCoords.x = Mathf.Min(texCoords.x, vec.x);
            texCoords.y = Mathf.Min(texCoords.y, vec.y);
            texCoords.width = Mathf.Max(texCoords.width, vec.x);
            texCoords.height = Mathf.Max(texCoords.height, vec.y);
        }

        Vector2 compSize = target.TextureSize;
        texCoords.width = (texCoords.width - texCoords.x) * bigTex.width;
        texCoords.height = (texCoords.height - texCoords.y) * bigTex.height;
        texCoords.x *= bigTex.width;
        texCoords.y *= bigTex.height;

        Graphics.CopyTexture(bigTex, 0, 0, (int)texCoords.x, (int)texCoords.y, (int)texCoords.width, (int)texCoords.height, newTex, 0, 0, 0, 0);

        if (target.Turns == 0) return newTex;
        if (target.Turns == 3) return RotateTexture(newTex, false);
        if (target.Turns == 1) return RotateTexture(newTex, true);
        else return RotateTexture(RotateTexture(newTex, false), false);
    }
}
#endif