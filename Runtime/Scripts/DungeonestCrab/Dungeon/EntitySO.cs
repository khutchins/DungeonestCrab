using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    [CreateAssetMenu(menuName = "Dungeon/Entity")]
    public class EntitySO : ScriptableObject {
        public string ID;
        [PreviewField(50, ObjectFieldAlignment.Right)]
        public GameObject Prefab;
        public bool BlocksMovement;
        [Tooltip("Whether or not the mesh can be merged. Use only if the object does not rotate or move at runtime [MOVEMENT INCLUDES BILLBOARDS].")]
        public bool CanBeMerged;

        [Header("Tile Modifiers")]
        public bool ReplacesCeiling;
        public bool ReplacesFloor;
        public bool RaiseToCeiling;

        [Header("Minimap")]
        public bool ShowOnMap;
        [ShowIf("ShowOnMap")]
        [Tooltip("Image to show on map. If no sprite is provided, no image will be placed on the minimap.")]
        public Sprite MapImage;
        [ShowIf("ShowOnMap")]
        public bool RotateMapImage;

        public GameObject Initialize(Vector2Int coords, Vector3 basePosition) {
            return Instantiate(Prefab, basePosition, Quaternion.identity);
        }
    }
}