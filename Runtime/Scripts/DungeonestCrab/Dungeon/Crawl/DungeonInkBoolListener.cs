using DungeonestCrab.Dungeon.Pen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    [DefaultExecutionOrder(1)]
    public abstract class DungeonBoolInkListener : MonoBehaviour {
        private string _trigger;
        private bool _invert;

        private bool _showing = true;
        private bool _started = false;
        private bool _forceUpdate = false;

        private void Start() {
            _started = true;
            Register();
        }

        public void SetTriggerAndInvert(string trigger, bool invert) {
            Unregister();
            _trigger = trigger;
            _invert = invert;
            Register();
        }

        private void Register() {
            if (_trigger == null) return;
            // Need to wait until after start so that it won't hide the interactable
            // before the interactable registers it in start.
            if (!_started) return;
            _forceUpdate = true;
            InkStateManager.INSTANCE.Manager.RegisterBoolVariableListener(_trigger, UpdatedTriggerValue, true);
        }

        private void Unregister() {
            InkStateManager.INSTANCE.Manager.UnregisterBoolVariableListener(_trigger, UpdatedTriggerValue);
        }

        private void OnEnable() {
            Register();
        }

        private void OnDisable() {
            Unregister();
        }

        protected abstract void OnStateChange(bool on);

        void UpdatedTriggerValue(bool value) {
            bool newVal = _showing;
            if (value == _invert) {
                newVal = false;
            } else {
                newVal = true;
            }
            if (newVal != _showing || _forceUpdate) {
                _showing = newVal;
                OnStateChange(_showing);
                _forceUpdate = false;
            }
        }
    }
}