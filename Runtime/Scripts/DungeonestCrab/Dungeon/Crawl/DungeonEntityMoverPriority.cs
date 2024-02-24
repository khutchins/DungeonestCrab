using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    public class DungeonEntityMoverPriority : DungeonEntityMover {
        [SerializeField] PriorityType Type;
        [Tooltip("Flips the action order.")]
        [SerializeField] bool _flipPriority;
        [Tooltip("Action cascade. At the bottom is an implicit DoNothing.")]
        [SerializeField] DungeonEntity.TurnAction[] _actions;

        public enum PriorityType {
            // Always checks in order from zero.
            ResetEachTurn,
            // Checks from the last success. If that fails, resets to zero and tries in order.
            Sticky,
            // Doesn't reset, moves to next priority on next request.
            Cycle,
            // Sticks with a move as long as it works, then moves to the next in the queue.
            StickyCycle
        }

        private int _lastIndex = 0;
        private bool _firstCycle = true;
        private PriorityType _originalPriority;

        protected override void ExtraAwake() {
            if (_flipPriority) {
                Array.Reverse(_actions);
            }
            _originalPriority = Type;
        }

        protected override void ExtraReset() {
            _lastIndex = 0;
            _firstCycle = true;
            Type = _originalPriority;
        }

        public override DungeonEntity.TurnAction GetTurnAction() {
            DungeonEntity.TurnAction action = DungeonEntity.TurnAction.DoNothing;
            switch (Type) {
                case PriorityType.ResetEachTurn:
                default:
                    _lastIndex = 0;
                    CycleStartingAt(0, out action);
                    break;
                case PriorityType.Sticky:
                    if (Entity.CanDoMove(_actions[_lastIndex])) {
                        action = _actions[_lastIndex];
                    } else {
                        _lastIndex = CycleStartingAt(0, out action);
                    }
                    break;
                case PriorityType.Cycle:
                    _lastIndex = CycleStartingAt(_firstCycle ? _lastIndex : _lastIndex + 1, out action);
                    break;
                case PriorityType.StickyCycle:
                    _lastIndex = CycleStartingAt(_lastIndex, out action);
                    break;
            }
            _firstCycle = false;
            return action;
        }

        private int CycleStartingAt(int startIdx, out DungeonEntity.TurnAction action) {
            for (int i = 0; i < _actions.Length; i++) {
                int compIdx = (i + startIdx) % _actions.Length;
                if (Entity.CanDoMove(_actions[compIdx])) {
                    action = _actions[compIdx];
                    return compIdx;
                }
            }
            action = DungeonEntity.TurnAction.DoNothing;
            return 0;
        }
    }
}