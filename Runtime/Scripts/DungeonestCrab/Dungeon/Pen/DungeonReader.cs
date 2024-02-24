using Ink.Runtime;
using KH;
using KH.Texts;
using Ratferences;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static KH.TaskQueue;

namespace DungeonestCrab.Dungeon.Pen {
    public class DungeonReader : MonoBehaviour {
        public static DungeonReader INSTANCE;

        [SerializeField] TaskQueue TaskQueue;
        [SerializeField] BoolReference MovementQueueingAllowed;
        [SerializeField] LineSpecQueue DefaultTextQueue;
        [SerializeField] TextQueue[] OtherTextQueues;
        protected IStoryReader _reader;

        private void Awake() {
            _reader = GenerateStoryReader();
            INSTANCE = this;
        }

        [System.Serializable]
        public class TextQueue {
            public string ID;
            public LineSpecQueue Queue;
        }

        protected readonly static string READER_CURRENT_QUEUE = "currentQueue";
        protected readonly static string READER_DEFAULT_QUEUE = "defaultQueue";
        protected readonly static string READER_AUTO_CONVO = "autoConvo";
        protected readonly static string QUEUE_TEXT = ">>> QUEUE:";

        protected virtual IStoryReader GenerateStoryReader(System.Func<TaskStoryReader, Story, string, List<string>, ITask> lineTaskGenerator) {
            TaskStoryReader reader = new TaskStoryReader(TaskQueue, lineTaskGenerator);
            reader.AddStoryStartCallback((reader) => {
                reader.ClearScratch(READER_AUTO_CONVO);
                reader.WriteScratch(READER_CURRENT_QUEUE, DefaultTextQueue);
            });
            reader.AddSpecialLineHandler(">>> AWAIT", (reader, line) => {
                // This naturally will clear the queue before continuing.
                return true;
            });
            reader.AddSpecialLineHandler(">>> AUTOCONVO", (reader, line) => {
                reader.WriteScratch(READER_AUTO_CONVO, true);
                return true;
            });
            reader.AddSpecialLineHandler((TaskStoryReader reader, string line) => {
                return line.StartsWith(QUEUE_TEXT);
            }, (TaskStoryReader reader, string line) => {
                string remainder = line.Substring(QUEUE_TEXT.Length).Trim();
                LineSpecQueue queue = DefaultTextQueue;
                if (remainder == "Default") {
                    // Special handling.
                } else {
                    TextQueue queuePair = OtherTextQueues.FirstOrDefault(x => x.ID == remainder);
                    if (queuePair == null) {
                        Debug.LogWarning($"LineSpecQueue with name: {remainder} not set!");
                    } else {
                        queue = queuePair.Queue;
                    }
                }

                reader.WriteScratch(READER_CURRENT_QUEUE, queue);
                return true;
            });
            reader.AddChoiceTweaker((reader, task) => {
                if ((bool)reader.ReadScratch(READER_AUTO_CONVO, false)) {
                    task.SelectedChoice = 0;
                }
                return task;
            });

            return reader;
        }

        protected virtual IStoryReader GenerateStoryReader() {
            return GenerateStoryReader((reader, story, line, tags) => {
                string[] segments = line.Split(':', 2);
                string speaker = segments.Length == 2 ? segments[0] : "";
                line = (segments.Length == 2 ? segments[1] : segments[0]).Trim();
                Color col = Color.white;

                return new TextTask(
                    reader.ReadScratch(READER_CURRENT_QUEUE, null) as LineSpecQueue,
                    new LineSpec(speaker.Trim(), line.Trim(), col)
                );
            });
        }

        public void PerformInkAction(string passage) {
            StartCoroutine(PerformInkActionCoroutine(passage));
        }

        public IEnumerator PerformInkActionCoroutine(string passage) {
            bool oldQueueing = MovementQueueingAllowed.Value;
            MovementQueueingAllowed.Value = false;
            yield return InkStateManager.INSTANCE.Manager.ReadPassage(passage, _reader);

            // This is necessary, as tasks can be enqueued after the last line,
            // and we should wait for those to finish too.
            yield return TaskQueue.WaitUntilEmpty();
            MovementQueueingAllowed.Value = oldQueueing;
        }
    }
}