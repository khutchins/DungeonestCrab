using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace DungeonestCrab.Dungeon {

    [Serializable]
    public class EntityMapMixin : EntityMixin {
        [Title("Minimap Settings")]
        [PreviewField(45, ObjectFieldAlignment.Right)]
        [Tooltip("Image to show on map. If no sprite is provided, no image will be placed on the minimap.")]
        public Sprite MapImage;

        public bool RotateMapImage;
    }
}