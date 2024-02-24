using DungeonestCrab.Dungeon.Pen;
using Ink.Runtime;
using KH;
using KH.Music;
using KH.Texts;
using Ratferences;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static KH.TaskQueue;

namespace DungeonestCrab.Dungeon {
    [DefaultExecutionOrder(1)]
    public class DungeonTaskHandlers : MonoBehaviour {
        [SerializeField] MusicSOList Tracks;
        [SerializeField] TaskQueue InteractQueue;
        [SerializeField] LineSpecQueue FaderTextQueue;
        [SerializeField] ColorReference BackgroundColor;
        [SerializeField] FloatReference FaderRef;

        private void Awake() {
            FaderRef.Value = 1f;

            Story story = InkStateManager.INSTANCE.InkStory;

            story.BindExternalFunction("setMusic", (string music) => {
                InteractQueue.Enqueue(new MusicTask(music));
            });
            story.BindExternalFunction("setFog", (string color, float density) => {
                InteractQueue.Enqueue(new FogTask(color, density));
            });
            story.BindExternalFunction("setSky", (string color) => {
                InteractQueue.Enqueue(new SkyColorTask(color));
            });
            story.BindExternalFunction("fadeIn", (bool faded, float duration) => {
                InteractQueue.Enqueue(new FadeTask(faded, duration));
            });
            story.BindExternalFunction("goToTitle", () => {
                InteractQueue.Enqueue(new SceneLoadTask("MenuScene"));
            });

            TaskQueueManager.INSTANCE.AddHandler(typeof(TextTask), TextTaskHandler);

            TaskQueueManager.INSTANCE.AddHandler(typeof(MusicTask), MusicTaskHandler);
            TaskQueueManager.INSTANCE.AddHandler(typeof(FogTask), FogTaskHandler);
            TaskQueueManager.INSTANCE.AddHandler(typeof(SkyColorTask), SkyColorTaskHandler);
            TaskQueueManager.INSTANCE.AddHandler(typeof(FadeTask), FadeTaskHandler);
            TaskQueueManager.INSTANCE.AddHandler(typeof(SceneLoadTask), SceneLoadTaskHandler);
        }

        IEnumerator FadeTaskHandler(ITask task) {
            FadeTask tt = task as FadeTask;
            float from = tt.FadeIn ? 1 : 0;
            float to = tt.FadeIn ? 0 : 1;
            yield return EZTween.DoPercentAction((float percent) => {
                FaderRef.Value = Mathf.Lerp(from, to, percent);
            }, tt.Duration);
        }

        IEnumerator SceneLoadTaskHandler(ITask task) {
            SceneLoadTask tt = task as SceneLoadTask;
            SceneManager.LoadScene(tt.SceneName);
            yield return null;
        }

        IEnumerator TextTaskHandler(ITask task) {
            TextTask tt = task as TextTask;
            yield return tt.Queue.EnqueueAndAwait(tt.Text);
        }

        IEnumerator MusicTaskHandler(ITask task) {
            MusicTask t = task as MusicTask;
            MusicSO theme = Tracks.GetOrWarn(t.Track, "Music track");
            if (theme != null) {
                MusicManager.INSTANCE.ClearAndSetDefault(theme.ToInfo());
            }
            yield break;
        }

        IEnumerator FogTaskHandler(ITask task) {
            FogTask t = task as FogTask;
            RenderSettings.fog = t.Density > 0;

            if (t.Density > 0) {
                RenderSettings.fog = true;
                RenderSettings.fogDensity = t.Density;
                RenderSettings.fogColor = t.Color;
            } else {
                RenderSettings.fog = false;
            }
            yield break;
        }

        IEnumerator SkyColorTaskHandler(ITask task) {
            SkyColorTask t = task as SkyColorTask;
            BackgroundColor.Value = t.Color;
            yield break;
        }
    }
}