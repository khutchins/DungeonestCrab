using Pomerandomian;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    /// <summary>
    /// A general action node that can modify Tile Type, Terrain, and Tags in a single configuration.
    /// Similar to the general Tile Matcher node.
    /// </summary>
    [CreateNodeMenu("Dungeon/Definitions/Actions/General Action")]
    public class TileActionNode : ActionProviderNode {
        
        [ToggleGroup("SetTile", "Set Tile Type")]
        public bool SetTile;
        [ToggleGroup("SetTile")]
        public Tile TileToSet;

        [ToggleGroup("SetTerrain", "Set Terrain")]
        public bool SetTerrain;
        [ToggleGroup("SetTerrain")]
        public TerrainSO TerrainToSet;

        [Header("Tags")]
        public List<string> TagsToAdd = new List<string>();
        public List<string> TagsToRemove = new List<string>();

        public override ITileAction GetAction() => new GeneralAction(
            SetTile, TileToSet, 
            SetTerrain, TerrainToSet, 
            TagsToAdd, TagsToRemove
        );

        private class GeneralAction : ITileAction {
            readonly bool _setTile, _setTerrain;
            readonly Tile _tile;
            readonly TerrainSO _terrain;
            readonly List<string> _add, _remove;

            public GeneralAction(bool setTile, Tile tile, bool setTerrain, TerrainSO terrain, List<string> add, List<string> remove) {
                _setTile = setTile;
                _tile = tile;
                _setTerrain = setTerrain;
                _terrain = terrain;
                _add = add ?? new List<string>();
                _remove = remove ?? new List<string>();
            }

            public void Apply(TileSpec spec, IRandom rand) {
                if (_setTile) spec.Tile = _tile;
                if (_setTerrain) spec.Terrain = _terrain;
                foreach (var t in _remove) spec.RemoveTag(t);
                foreach (var t in _add) spec.AddTag(t);
            }
        }
    }
}
