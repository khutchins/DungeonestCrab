using Ink.Runtime;
using KH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KH.TaskQueue;

namespace DungeonestCrab.Dungeon.Pen {
    public class ChoiceTask : ITask {
        public readonly List<Choice> Choices;
        public int SelectedChoice = -1;

        public ChoiceTask(List<Choice> choices) {
            Choices = choices;
        }

        public bool ChoiceMade { get => SelectedChoice >= 0; }
    }

    public class TaskStoryReader : IStoryReader {
        private readonly TaskQueue _taskQueue;
        private readonly List<IChoiceTweaker> _choiceTweakers = new List<IChoiceTweaker>();
        private readonly List<ISpecialLineHandler> _handlers = new();
        private readonly List<ITagHandler> _tagHandlers = new();
        private readonly Func<Story, string, List<string>, ITask> _lineTaskGenerator;
        private readonly Dictionary<string, object> _scratch = new Dictionary<string, object>();

        public TaskStoryReader(TaskQueue queue, Func<Story, string, List<string>, ITask> lineTaskGenerator) {
            _taskQueue = queue;
            _lineTaskGenerator = lineTaskGenerator;
        }

        public object ReadScratch(string key, object defaultValue) {
            if (_scratch.ContainsKey(key)) return _scratch[key];
            return defaultValue;
        }

        public void WriteScratch(string key, object value) {
            _scratch[key] = value;
        }

        public ISpecialLineHandler AddSpecialLineHandler(string exactLine, Func<string, bool> handle) {
            ISpecialLineHandler handler = CallbackLineHandler.ForExactLine(exactLine, handle);
            AddSpecialLineHandler(handler);
            return handler;
        }

        public ISpecialLineHandler AddSpecialLineHandler(Func<string, bool> shouldHandle, Func<string, bool> handle) {
            ISpecialLineHandler handler = new CallbackLineHandler(shouldHandle, handle);
            AddSpecialLineHandler(handler);
            return handler;
        }

        public void AddSpecialLineHandler(ISpecialLineHandler handler) {
            _handlers.Remove(handler);
            _handlers.Add(handler);
        }

        public ITagHandler AddTagHandler(string exactLine, Action<string> handle) {
            ITagHandler handler = CallbackTagHandler.ForExactLine(exactLine, handle);
            AddTagHandler(handler);
            return handler;
        }

        public ITagHandler AddTagHandler(Func<string, bool> shouldHandle, Action<string> handle) {
            ITagHandler handler = new CallbackTagHandler(shouldHandle, handle);
            AddTagHandler(handler);
            return handler;
        }

        public void AddTagHandler(ITagHandler handler) {
            _tagHandlers.Remove(handler);
            _tagHandlers.Add(handler);
        }

        public IChoiceTweaker AddChoiceTweaker(Func<ChoiceTask, ChoiceTask> tweaker) {
            var handler = new CallbackChoiceTweaker(tweaker);
            AddChoiceTweaker(handler);
            return handler;
        }

        public void AddChoiceTweaker(IChoiceTweaker tweaker) {
            _choiceTweakers.Remove(tweaker);
            _choiceTweakers.Add(tweaker);
        }

        public IEnumerator HandleStory(Story story) {
            while (story.canContinue) {
                while (story.canContinue) {
                    string line = story.Continue();

                    foreach (string tag in story.currentTags) {
                        foreach (ITagHandler handler in _tagHandlers) {
                            if (handler.ShouldHandle(tag)) handler.Handle(tag);
                        }
                    }

                    // Can happen when setting variables. Since it shouldn't happen otherwise,
                    // always "drop" the line.
                    if (line.Length == 0) continue;

                    bool skipLine = false;
                    foreach (ISpecialLineHandler handler in _handlers) {
                        if (handler.ShouldHandle(line) && handler.Handle(line)) {
                            // Line processing should stop.
                            skipLine = true;
                            continue;
                        }
                    }
                    if (skipLine) {
                        _taskQueue.WaitUntilEmpty();
                    } else {
                        ITask task = _lineTaskGenerator(story, line, story.currentTags);
                        yield return _taskQueue.EnqueueAndAwaitTaskFinished(task);
                    }
                }

                if (story.currentChoices.Count > 0) {
                    ChoiceTask task = new ChoiceTask(story.currentChoices);
                    foreach (IChoiceTweaker tweaker in _choiceTweakers) {
                        task = tweaker.TweakChoices(task);
                    }
                    if (!task.ChoiceMade) {
                        yield return _taskQueue.EnqueueAndAwaitTaskFinished(task);
                    }
                    story.ChooseChoiceIndex(task.SelectedChoice);
                }
            }

            // This is necessary, as tasks can be enqueued after the last line,
            // and we should wait for those to finish too.
            yield return _taskQueue.WaitUntilEmpty();
        }

        public class CallbackLineHandler : ISpecialLineHandler {
            private readonly Func<string, bool> _shouldHandle;
            private readonly Func<string, bool> _handle;

            public CallbackLineHandler(Func<string, bool> shouldHandle, Func<string, bool> handle) {
                _shouldHandle = shouldHandle;
                _handle = handle;
            }

            public bool Handle(string line) {
                return _handle(line);
            }

            public bool ShouldHandle(string line) {
                return _shouldHandle(line);
            }

            public static CallbackLineHandler ForExactLine(string lineToMatch, Func<string, bool> handle) {
                return new CallbackLineHandler((line) => line == lineToMatch, handle);
            }
        }

        public class CallbackTagHandler : ITagHandler {
            private readonly Func<string, bool> _shouldHandle;
            private readonly Action<string> _handle;

            public CallbackTagHandler(Func<string, bool> shouldHandle, Action<string> handle) {
                _shouldHandle = shouldHandle;
                _handle = handle;
            }

            public void Handle(string line) {
                _handle(line);
            }

            public bool ShouldHandle(string line) {
                return _shouldHandle(line);
            }

            public static CallbackTagHandler ForExactLine(string lineToMatch, Action<string> handle) {
                return new CallbackTagHandler((line) => line == lineToMatch, handle);
            }
        }

        public class CallbackChoiceTweaker : IChoiceTweaker {
            private readonly Func<ChoiceTask, ChoiceTask> _handler;

            public CallbackChoiceTweaker(Func<ChoiceTask, ChoiceTask> handler) {
                _handler = handler;
            }

            public ChoiceTask TweakChoices(ChoiceTask task) {
                return _handler.Invoke(task);
            }
        }

        public interface ISpecialLineHandler {
            /// <summary>
            /// Whether or not the handler handles the line.
            /// </summary>
            public bool ShouldHandle(string line);

            /// <summary>
            /// Handles the line. If it gets in here, ShouldHandle returned true.
            /// </summary>
            /// <param name="line">Line to process</param>
            /// <returns>true if the line should be dropped, false otherwise.</returns>
            public bool Handle(string line);
        }

        public interface ITagHandler {
            /// <summary>
            /// Whether or not the handler handles the tag.
            /// </summary>
            public bool ShouldHandle(string tag);
            /// <summary>
            /// Handles the tag.
            /// </summary>
            public void Handle(string tag);
        }

        public interface IChoiceTweaker {
            /// <summary>
            /// Make any modification to the ChoiceTask, including generating a new
            /// one or automatically selecting a value. Must return a non-null ChoiceTask.
            ///
            /// If an option is already selected, it will skip sending the choice out.
            /// </summary>
            public ChoiceTask TweakChoices(ChoiceTask task);
        }
    }
}