using Ink.Runtime;
using KH;
using KH.Texts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Pen {
    public class InkStateManager : MonoBehaviour {
        [SerializeField] TextAsset InkFile;

        public static InkStateManager INSTANCE;
        [SerializeField] protected TaskQueue InteractQueue;
        private StoryManager _manager;

        public class TitlePassage {
            public readonly string Title;
            public readonly string Passage;

            public TitlePassage(string title, string passage) {
                Title = title;
                Passage = passage;
            }

            public override bool Equals(object obj) {
                return obj is TitlePassage passage &&
                       Title == passage.Title &&
                       Passage == passage.Passage;
            }

            public override int GetHashCode() {
                return HashCode.Combine(Title, Passage);
            }
        }

        public StoryManager Manager => _manager;

        public Story InkStory {
            get => _manager.Story;
        }

        public class EphemeralInkPassage : IDisposable {
            private static int flowIncrementer = 1;
            private string _flowId;
            private string _cachedFlow = null;
            private Story _story;

            public EphemeralInkPassage(Story story, string passage) {
                _story = story;
                _flowId = $"f{flowIncrementer++}";
                if (!_story.currentFlowIsDefaultFlow) _cachedFlow = _story.currentFlowName;
                _story.SwitchFlow(_flowId);
                _story.ChoosePathString(passage);
            }

            public void Dispose() {
                if (_cachedFlow != null) {
                    _story.SwitchFlow(_cachedFlow);
                }
                _story.RemoveFlow(_flowId);
            }
        }

        void Awake() {
            _manager = new StoryManager(InkFile);
            OnAwake();
            INSTANCE = this;
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

        /// <summary>
        /// Expected format: #rrggbb or #rrggbbaa
        /// </summary>
        public static Color StringToColor(string str) {
            float r = int.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            float g = int.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            float b = int.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            float a = str.Length > 7 ? int.Parse(str.Substring(7, 2), System.Globalization.NumberStyles.HexNumber) : 255;
            return new Color(r / 255, g / 255, b / 255, a / 255);
        }

    }
}