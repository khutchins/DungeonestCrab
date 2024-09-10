using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITextureView {
    Material Material { get; }
    Vector2[] UV { get; }
}
