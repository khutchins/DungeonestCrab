using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    public class DungeonEntityMoverHunt : DungeonEntityMover {
        public enum State {
            Wait,
            ReturnHome,
            Hunt
        }
        public State HuntState = State.Hunt;
        private State _startState;

        protected override void ExtraAwake() {
            _startState = HuntState;
        }

        protected override void ExtraReset() {
            HuntState = _startState;
        }

        public void SetHunt() {
            HuntState = State.Hunt;
        }

        public void SetReturnHome() {
            HuntState = State.ReturnHome;
        }

        public void SetWait() {
            HuntState = State.Wait;
        }

        public override DungeonEntity.TurnAction GetTurnAction() {
            DungeonEntity.TurnAction action = DungeonEntity.TurnAction.DoNothing;

            DungeonInteractable interact = this.GetComponent<DungeonInteractable>();
            action = HuntState switch {
                State.Wait => DungeonEntity.TurnAction.DoNothing,
                State.ReturnHome => DungeonGrid.INSTANCE.MoveForPathToNode(interact, _home),
                State.Hunt => DungeonGrid.INSTANCE.HuntPlayer(interact),
                _ => DungeonEntity.TurnAction.DoNothing,
            };
            return action;
        }
    }
}