using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    [CreateAssetMenu(menuName = "DungeonestCrab/Entity")]
    public class EntitySO : ScriptableObject {
        public EntitySpec Entity;
    }

    [System.Serializable]
    public class EntitySpec {
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

        public class Builder {
            private string _id;
            private GameObject _prefab;
            private bool _blocksMovement = false;
            private bool _canBeMerged = false;
            private bool _replacesCeiling = false;
            private bool _replacesFloor = false;
            private bool _raiseToCeiling = false;
            private bool _showOnMap;
            private Sprite _mapImage;
            private bool _rotateMapImage;

            public Builder(string ID, GameObject prefab) {
                _id = ID;
                _prefab = prefab;
            }

            public Builder SetBlocksMovement(bool blocksMovement = true) {
                _blocksMovement = blocksMovement;
                return this;
            }

            public Builder SetCanBeMerged(bool canBeMerged = true) {
                _canBeMerged = canBeMerged;
                return this;
            }

            public Builder SetReplacesCeiling(bool replacesCeiling = true) {
                _replacesCeiling = replacesCeiling;
                return this;
            }

            public Builder SetReplacesFloor(bool replacesFloor = true) {
                _replacesFloor = replacesFloor;
                return this;
            }

            public Builder SetRaiseToCeiling(bool raiseToCeiling = true) {
                _raiseToCeiling = raiseToCeiling;
                return this;
            }

            public Builder SetShowOnMap(Sprite mapSprite, bool rotateImageOnMap) {
                _showOnMap = true;
                _mapImage = mapSprite;
                _rotateMapImage = rotateImageOnMap;
                return this;
            }

            public EntitySpec Build() {
                return new EntitySpec {
                    ID = _id,
                    Prefab = _prefab,
                    BlocksMovement = _blocksMovement,
                    CanBeMerged = _canBeMerged,
                    ReplacesCeiling = _replacesCeiling,
                    ReplacesFloor = _replacesFloor,
                    RaiseToCeiling = _raiseToCeiling,
                    ShowOnMap = _showOnMap,
                    MapImage = _mapImage,
                    RotateMapImage = _rotateMapImage
                };
            }
        }
    }
}