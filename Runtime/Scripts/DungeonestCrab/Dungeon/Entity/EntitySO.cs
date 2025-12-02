using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    [Serializable]
    public abstract class EntityMixin { }

    [CreateAssetMenu(menuName = "DungeonestCrab/Entity")]
    public class EntitySO : ScriptableObject {
        public EntitySpec Entity;
    }

    [System.Serializable]
    public class EntitySpec {
        public string ID;
        [PreviewField(50, ObjectFieldAlignment.Right)]
        public GameObject Prefab;
        public string[] Tags;
        public bool BlocksMovement;
        [Tooltip("Whether or not the mesh can be merged. Use only if the object does not rotate or move at runtime [MOVEMENT INCLUDES BILLBOARDS].")]
        public bool CanBeMerged;

        [Title("Features")]
        [SerializeReference]
        [ListDrawerSettings(ShowIndexLabels = false, ShowFoldout = true)]
        public List<EntityMixin> Mixins = new List<EntityMixin>();

        [Header("Tile Modifiers")]
        public bool ReplacesCeiling;
        public bool ReplacesFloor;
        public bool RaiseToCeiling;

        public bool HasTag(string tag) {
            return Tags.Contains(tag);
        }

        public T GetMixin<T>() where T : EntityMixin {
            for (int i = 0; i < Mixins.Count; i++) {
                if (Mixins[i] is T t) return t;
            }
            return null;
        }

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
                var spec = new EntitySpec {
                    ID = _id,
                    Prefab = _prefab,
                    BlocksMovement = _blocksMovement,
                    CanBeMerged = _canBeMerged,
                    ReplacesCeiling = _replacesCeiling,
                    ReplacesFloor = _replacesFloor,
                    RaiseToCeiling = _raiseToCeiling,
                };
                if (_showOnMap) {
                    spec.Mixins.Add(new EntityMapMixin {
                        RotateMapImage = _rotateMapImage,
                        MapImage = _mapImage
                    });
                }
                return spec;
            }
        }
    }
}