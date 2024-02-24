using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSetter : MonoBehaviour {

    public void SetSprite(Texture texture) {
        if (texture == null) return;
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            MaterialPropertyBlock prop = new();
            prop.SetTexture("_MainTex", texture);
            renderer.SetPropertyBlock(prop);
        }
    }

    public void SetDistance(float distance) {
        foreach (Transform child in this.transform) {
            child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, distance);
        }
    }
}
