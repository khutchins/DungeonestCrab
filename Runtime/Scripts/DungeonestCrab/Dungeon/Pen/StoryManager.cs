using Ink.Runtime;
using KH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Pen {

    public interface IStoryReader {
        IEnumerator HandleStory(Story story);
    }

    public class CallbackStoryReader : IStoryReader {

        private readonly Func<Story, IEnumerator> _storyHandler;

        public CallbackStoryReader(Func<Story, IEnumerator> storyHandler) {
            _storyHandler = storyHandler;
        }

        public IEnumerator HandleStory(Story story) {
            return _storyHandler(story);
        }

    }

    public class StoryManager {

        private Story _story;
        private readonly Dictionary<string, HashSet<Action<bool>>> _boolVariableListeners = new();
        private readonly Dictionary<string, HashSet<Action<int>>> _intVariableListeners = new();
        private readonly Dictionary<string, HashSet<Action<string>>> _stringVariableListeners = new();

        public delegate void ResetTriggered(Story story);
        public ResetTriggered OnReset;

        public Story Story { get => _story; }
        private IStoryReader _defaultReader;

        public StoryManager(TextAsset textAsset) : this(textAsset.text) {
        }

        public StoryManager(string text) {
            _story = new Story(text);
            _story.variablesState.variableChangedEvent += VariableChanged;

            _story.onError += (msg, type) => {
                if (type == Ink.ErrorType.Warning) Debug.LogWarning(msg);
                else Debug.LogError(msg);
            };
        }

        public void SetDefaultReader(Func<Story, IEnumerator> handler) {
            SetDefaultReader(new CallbackStoryReader(handler));
        }

        public void SetDefaultReader(IStoryReader reader) {
            _defaultReader = reader;
        }

        public bool SetPassage(string passage) {
            if (!IsValidKnotOrStitch(passage)) {
                Debug.LogWarning($"Invalid knot/stitch {passage}! Not processing.");
                return false;
            }
            _story.ChoosePathString(passage);
            return true;
        }


        public IEnumerator Read() {
            yield return Read(_defaultReader);
        }

        public IEnumerator Read(Func<Story, IEnumerator> handler) {
            return Read(new CallbackStoryReader(handler));
        }

        public IEnumerator Read(IStoryReader reader) {
            if (reader == null) {
                Debug.LogWarning("Attempting to process passage with invalid reader.");
                yield break;
            }

            yield return reader.HandleStory(_story);
        }

        public IEnumerator ReadPassage(string passage) {
            return ReadPassage(passage, _defaultReader);
        }

        public IEnumerator ReadPassage(string passage, Func<Story, IEnumerator> handler) {
            return ReadPassage(passage, new CallbackStoryReader(handler));
        }

        public IEnumerator ReadPassage(string passage, IStoryReader reader) {
            if (!SetPassage(passage)) {
                yield break;
            }

            _story.ChoosePathString(passage);
            yield return Read(reader);
        }

        public void SilentlyRunPassageUntilFirstChoiceOrEnd(string passage) {
            if (!IsValidKnotOrStitch(passage)) {
                Debug.LogWarning($"Failed running {passage} silently. It doesn't exist!");
            }
            _story.ChoosePathString(passage);
            while (_story.canContinue) {
                _story.Continue();
            }
        }

        /// <summary>
        /// Saves the ink state on creation, restores it on dispose.
        /// Use it inside of a using block, like `using (HandleFlow(passage)) {}`
        /// </summary>
        public EphemeralInkPassage HandleFlow(string passage) {
            return new EphemeralInkPassage(_story, passage);
        }

        public static bool IsValidKnotOrStitch(Story story, string name) {
            if (name == null) return false;
            return story.ContentAtPath(new Ink.Runtime.Path(name)).correctObj != null;
        }

        public bool IsValidKnotOrStitch(string name) {
            return IsValidKnotOrStitch(_story, name);
        }

        public int VisitCount(string passageName) {
            if (string.IsNullOrEmpty(passageName)) return 0;
            return _story.state.VisitCountAtPathString(passageName);
        }

        public string Save() {
            return _story.state.ToJson();
        }

        public void SaveTo(string filePath, string json) {
            string tmp = $"{filePath}.tmp";
            if (File.Exists(filePath)) {
                IOHelper.EnsurePathAndWriteText(tmp, json);
                File.Replace(tmp, filePath, $"{filePath}.bak");
            } else {
                IOHelper.EnsurePathAndWriteText(filePath, json);
            }
        }

        public void Load(string json) {
            _story.state.LoadJson(json);
        }

        public void LoadFrom(string filePath) {
            Load(File.ReadAllText(filePath));
        }

        public void Reset() {
            // I don't know for sure that we have to reset the listener, but
            // we probably should anyway.
            _story.variablesState.variableChangedEvent -= VariableChanged;
            _story.ResetState();
            _story.variablesState.variableChangedEvent += VariableChanged;
            OnReset?.Invoke(_story);
        }

        public InkList GetInkListVariable(string variableName) {
            return (InkList)_story.variablesState[variableName];
        }

        public void SetInkListVariable(string variableName, InkList value) {
            _story.variablesState[variableName] = value;
        }

        public void SetInkListVariable(string variableName, InkListItem value) {
            _story.variablesState[variableName] = ListWithItem(value);
        }

        public Ink.Runtime.Object GetVariable(string variableName) {
            return _story.variablesState.GetVariableWithName(variableName);
        }

        public VariablesState AllVariables { get => _story.variablesState; }

        public bool GetBoolVariable(string variableName, bool defaultValue = false) {
            Ink.Runtime.Object obj = _story.variablesState.GetVariableWithName(variableName);
            if (!(obj is BoolValue)) {
                Debug.LogWarning($"Requested ink variable {variableName} is not a bool!");
                return defaultValue;
            }
            return (obj as BoolValue).value;
        }

        public string GetStringVariable(string variableName, string defaultValue = null) {
            Ink.Runtime.Object obj = _story.variablesState.GetVariableWithName(variableName);
            if (!(obj is StringValue)) {
                Debug.LogWarning($"Requested ink variable {variableName} is not a string!");
                return defaultValue;
            }
            return (obj as StringValue).value;
        }

        public void SetStringVariable(string variableName, string value) {
            _story.variablesState[variableName] = value;
        }

        public int GetIntVariable(string variableName, int defaultValue = -1) {
            Ink.Runtime.Object obj = _story.variablesState.GetVariableWithName(variableName);
            if (!(obj is IntValue)) {
                Debug.LogWarning($"Requested ink variable {variableName} is not an int!");
                return defaultValue;
            }
            return (obj as IntValue).value;
        }

        public float GetFloatVariable(string variableName, float defaultValue = -1) {
            Ink.Runtime.Object obj = _story.variablesState.GetVariableWithName(variableName);
            if (!(obj is FloatValue)) {
                Debug.LogWarning($"Requested ink variable {variableName} is not an int!");
                return defaultValue;
            }
            return (obj as FloatValue).value;
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
        /// InkList having exactly one item. Useful for creating single item
        /// lists for function calls.
        /// </summary>
        /// <param name="list">InkList to spread.</param>
        /// <returns>List of InkLists with one entry each.</returns>
        public List<InkList> SpreadList(InkList list) {
            return list.Select(x => ListWithItem(x.Key)).ToList();
        }

        public InkList ListWithItem(InkListItem item) {
            InkList list = new InkList(item.originName, _story);
            list.AddItem(item);
            return list;
        }

        public void SetIntVariable(string variableName, int value) {
            _story.variablesState[variableName] = value;
        }

        public void SetFloatVariable(string variableName, float value) {
            _story.variablesState[variableName] = value;
        }

        public void SetBoolVariable(string variableName, bool value) {
            _story.variablesState[variableName] = value;
        }

        public void IncrementInkVariable(string variableName, int value) {
            _story.variablesState[variableName] = (int)_story.variablesState[variableName] + value;
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
            if (!_story.variablesState.GlobalVariableExistsWithName(variableName)) {
                Debug.LogWarning($"Caller trying to register listener for variable not in ink: {variableName}.");
                return;
            }
            if (immediateCallback) {
                listener((T)_story.variablesState[variableName]);
            }
        }

        private void UnregisterVariableListener<T>(Dictionary<string, HashSet<Action<T>>> dict, string variableName, Action<T> listener) {
            if (variableName == null) return;
            if (!dict.ContainsKey(variableName)) return;

            dict[variableName].Remove(listener);
        }

        void VariableChanged(string name, Ink.Runtime.Object value) {
            if (value is BoolValue) {
                HandleVariableChanged(_boolVariableListeners, name, (value as BoolValue).value);
            } else if (value is IntValue) {
                HandleVariableChanged(_intVariableListeners, name, (value as IntValue).value);
            } else if (value is StringValue) {
                HandleVariableChanged(_stringVariableListeners, name, (value as StringValue).value);
            }
        }

        private void HandleVariableChanged<T>(Dictionary<string, HashSet<Action<T>>> dict, string variableName, T value) {
            if (!dict.ContainsKey(variableName)) return;

            var listeners = dict[variableName];
            foreach (var listener in listeners.ToList()) {
                listener(value);
            }
        }

        /// <summary>
        /// Creates a disposable ink flow and switches to it. Do not
        /// process this and another flow or story at the same time,
        /// as it will not work.
        /// Use it inside of a using block, like `using (new EphemeralInkPassage(story, passage)) {}`
        /// </summary>
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
    }
}