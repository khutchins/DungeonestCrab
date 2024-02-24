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
        private Story _inkStory;
        private readonly Dictionary<string, HashSet<Action<bool>>> _boolVariableListeners = new();
        private readonly Dictionary<string, HashSet<Action<int>>> _intVariableListeners = new();
        private readonly Dictionary<string, HashSet<Action<string>>> _stringVariableListeners = new();

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

        public Story InkStory {
            get => _inkStory;
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

        public static bool IsValidKnotOrStitch(Story story, string name) {
            if (name == null) return false;
            return story.ContentAtPath(new Ink.Runtime.Path(name)).correctObj != null;
        }

        public int VisitCount(string passageName) {
            if (string.IsNullOrEmpty(passageName)) return 0;
            return _inkStory.state.VisitCountAtPathString(passageName);
        }

        public string Save() {
            return _inkStory.state.ToJson();
        }

        /// <summary>
        /// Saves the ink state on creation, restores it on dispose.
        /// Use it inside of a using block, like `using (HandleFlow(passage)) {}`
        /// </summary>
        public EphemeralInkPassage HandleFlow(string passage) {
            return new EphemeralInkPassage(_inkStory, passage);
        }

        public void SaveTo(string filePath, string json) {
            string tmp = $"{filePath}.tmp";
            if (File.Exists(filePath)) {
                IOHelper.EnsurePathAndWriteText(tmp, json);
                // Can't replace or overwrite because we want a new last modified date.
                File.Delete(filePath);
                File.Move(tmp, filePath);
            } else {
                IOHelper.EnsurePathAndWriteText(filePath, json);
            }
        }

        public void Load(string json) {
            _inkStory.state.LoadJson(json);
        }

        public void LoadFrom(string filePath) {
            Load(File.ReadAllText(filePath));
        }

        void Awake() {
            _inkStory = new Story(InkFile.text);
            _inkStory.variablesState.variableChangedEvent += VariableChanged;

            _inkStory.onError += (msg, type) => {
                if (type == Ink.ErrorType.Warning) Debug.LogWarning(msg);
                else Debug.LogError(msg);
            };

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
            if (!IsValidKnotOrStitch(_inkStory, passage)) {
                Debug.LogWarning($"Failed running {passage} silently. It doesn't exist!");
            }
            _inkStory.ChoosePathString(passage);
            while (_inkStory.canContinue) {
                _inkStory.Continue();
            }
        }

        public void Reset() {
            // I don't know for sure that we have to reset the listener, but
            // we probably should anyway.
            _inkStory.variablesState.variableChangedEvent -= VariableChanged;
            _inkStory.ResetState();
            OnReset();
            _inkStory.variablesState.variableChangedEvent += VariableChanged;
        }

        void VariableChanged(string name, Ink.Runtime.Object value) {
            if (value is BoolValue) {
                HandleVariable(_boolVariableListeners, name, (value as BoolValue).value);
            } else if (value is IntValue) {
                HandleVariable(_intVariableListeners, name, (value as IntValue).value);
            } else if (value is StringValue) {
                HandleVariable(_stringVariableListeners, name, (value as StringValue).value);
            }
        }

        private void HandleVariable<T>(Dictionary<string, HashSet<Action<T>>> listeners, string variableName, T value) {
            if (!listeners.ContainsKey(name)) return;

            var listenersForName = listeners[name];
            foreach (var listener in listenersForName.ToList()) {
                listener(value);
            }
        }

        public InkList GetInkListVariable(string variableName) {
            return (InkList)_inkStory.variablesState[variableName];
        }

        public string GetStringVariable(string variableName, string defaultValue = null) {
            Ink.Runtime.Object obj = _inkStory.variablesState.GetVariableWithName(variableName);
            if (!(obj is StringValue)) {
                Debug.LogWarning($"Requested ink variable {variableName} is not a string!");
                return defaultValue;
            }
            return (obj as StringValue).value;
        }

        public void SetStringVariable(string variableName, string value) {
            _inkStory.variablesState[variableName] = value;
        }

        public int GetIntVariable(string variableName, int defaultValue = -1) {
            Ink.Runtime.Object obj = _inkStory.variablesState.GetVariableWithName(variableName);
            if (!(obj is IntValue)) {
                Debug.LogWarning($"Requested ink variable {variableName} is not an int!");
                return defaultValue;
            }
            return (obj as IntValue).value;
        }

        /// <summary>
        /// Expected format: #rrggbb or #rrggbbaa
        /// </summary>
        public Color GetColorVariable(string variableName, Color defaultValue) {
            string str = GetStringVariable(variableName, null);
            if (str == null) return defaultValue;
            return StringToColor(str);
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

        /// <summary>
        /// Spreads an InkList into a List of InkLists, with each generated
        /// InkList having exactly one item.
        /// </summary>
        /// <param name="list">InkList to spread.</param>
        /// <returns>List of InkLists with one entry each.</returns>
        public List<InkList> SpreadList(InkList list) {
            return list.Select(x => {
                InkListItem item = x.Key;
                var subList = new InkList(item.originName, _inkStory);
                subList.AddItem(item);
                return subList;
            }).ToList();
        }

        public void SetIntVariable(string variableName, int value) {
            _inkStory.variablesState[variableName] = value;
        }

        public void SetBoolVariable(string variableName, bool value) {
            _inkStory.variablesState[variableName] = value;
        }

        public void IncrementInkVariable(string variableName, int value) {
            _inkStory.variablesState[variableName] = (int)_inkStory.variablesState[variableName] + value;
        }

        public void RegisterBoolVariableListener(string variableName, Action<bool> listener, bool immediateCallback = false) {
            RegisterVariableListener(_boolVariableListeners, variableName, listener, immediateCallback);
        }

        public void UnregisterBoolVariableListener(string variableName, Action<bool> listener) {
            UnregisterVariableListener(_boolVariableListeners, variableName, listener);
        }

        public void RegisterIntVariableListener(string variableName, Action<int> listener, bool immediateCallback = false) {
            RegisterVariableListener(_intVariableListeners, variableName, listener, immediateCallback);
        }

        public void UnregisterIntVariableListener(string variableName, Action<int> listener) {
            UnregisterVariableListener(_intVariableListeners, variableName, listener);
        }

        public void RegisterStringVariableListener(string variableName, Action<string> listener, bool immediateCallback = false) {
            RegisterVariableListener(_stringVariableListeners, variableName, listener, immediateCallback);
        }

        public void UnregisterStringVariableListener(string variableName, Action<string> listener) {
            UnregisterVariableListener(_stringVariableListeners, variableName, listener);
        }

        private void RegisterVariableListener<T>(Dictionary<string, HashSet<Action<T>>> dict, string variableName, Action<T> listener, bool immediateCallback = false) {
            if (variableName == null) {
                Debug.LogWarning("Caller trying to register listener for null variable.");
                return;
            }
            if (!dict.ContainsKey(variableName)) dict[variableName] = new();
            dict[variableName].Add(listener);
            if (!_inkStory.variablesState.GlobalVariableExistsWithName(variableName)) {
                Debug.LogWarning($"Caller trying to register listener for variable not in ink: {variableName}.");
                return;
            }
            if (immediateCallback) {
                listener((T)_inkStory.variablesState[variableName]);
            }
        }

        private void UnregisterVariableListener<T>(Dictionary<string, HashSet<Action<T>>> dict, string variableName, Action<T> listener) {
            if (variableName == null) return;
            if (!dict.ContainsKey(variableName)) return;

            dict[variableName].Remove(listener);
        }
    }
}