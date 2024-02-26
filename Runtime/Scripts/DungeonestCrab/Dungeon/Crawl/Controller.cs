using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KH.Input;
using Sirenix.OdinInspector;

namespace DungeonestCrab.Dungeon.Crawl {
    [CreateAssetMenu(menuName = "DungeonestCrab/Crawl/Controller")]
    public class Controller : ScriptableObject {
        [InlineEditor] public SingleInputMediator Interact;
        [InlineEditor] public SingleInputMediator MoveLeft;
        [InlineEditor] public SingleInputMediator MoveRight;
        [InlineEditor] public SingleInputMediator MoveForward;
        [InlineEditor] public SingleInputMediator MoveBack;
        [InlineEditor] public SingleInputMediator TurnLeft;
        [InlineEditor] public SingleInputMediator TurnRight;
        [InlineEditor] public CurrentInputSO CurrentInput;
    }
}