using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "DungeonestCrab/Spec/Source - Multi Options")]
    public class SourceComboSO : SourceSO {
        public enum SourceType {
            All = 0,
            Maze = 1,
            MazeHaphazard = 2,
        }

        [SerializeField] SourceType Type;

        [ShowIf("Type", SourceType.Maze)]
        [SerializeField] SourceMaze SourceMaze;

        [ShowIf("Type", SourceType.MazeHaphazard)]
        [SerializeField] SourceMazeHaphazard SourceMazeHaphazard;

        public override ISource ToSource() {
            switch (Type) {
                case SourceType.All:
                    return new SourceAll(TileToSet);
                case SourceType.Maze:
                    return SourceMaze;
                case SourceType.MazeHaphazard:
                    return SourceMazeHaphazard;
            }

            Debug.LogWarning($"Unhandled bounds type {Type}. Falling back to all.");
            return new SourceAll(TileToSet);
        }
    }
}