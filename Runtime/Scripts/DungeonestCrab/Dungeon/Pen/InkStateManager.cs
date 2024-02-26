using Ink.Runtime;
using KH;
using System;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Pen {
    public interface IInkFunctionRegistrar {
        public void RegisterOn(Story story, TaskQueue taskQueue);
    }

    public class InkStateManager : MonoBehaviour {
        [SerializeField] TextAsset InkFile;

        public static InkStateManager INSTANCE;
        [SerializeField] protected TaskQueue InteractQueue;
        private StoryManager _manager;

        public StoryManager Manager => _manager;

        public Story InkStory {
            get => _manager.Story;
        }

        void Awake() {
            if (InkFile == null) {
                Debug.LogWarning($"No ink file specified!");
                this.enabled = false;
                return;
            }
            _manager = new StoryManager(InkFile);
            INSTANCE = this;
            OnAwake();

            foreach (IInkFunctionRegistrar registrar in GetComponents<IInkFunctionRegistrar>()) {
                registrar.RegisterOn(_manager.Story, InteractQueue);
            }
        }

        private void Start() {
            OnStart();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnReset() { }

        /// <summary>
        /// Runs a passage without generating any text tasks.
        /// </summary>
        protected void SilentlyRunPassage(string passage) {
            _manager.SilentlyRunPassageUntilFirstChoiceOrEnd(passage);
        }

        public void Reset() {
            _manager.Reset();
            OnReset();
        }
    }
}