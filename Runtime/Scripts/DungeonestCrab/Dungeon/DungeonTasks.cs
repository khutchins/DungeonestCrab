using DungeonestCrab.Dungeon.Pen;
using KH.Texts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KH.TaskQueue;

namespace DungeonestCrab.Dungeon {
    public class TextTask : ITask {
        public readonly LineSpecQueue Queue;
        public readonly LineSpec Text;

        public TextTask(LineSpecQueue queue, LineSpec text) {
            Queue = queue;
            Text = text;
        }
    }


    public class MusicTask : ITask {
        public readonly string Track;

        public MusicTask(string track) {
            Track = track;
        }
    }

    public class FogTask : ITask {
        public readonly Color Color;
        public readonly float Density;

        public FogTask(string color, float density) {
            Color = StoryManager.StringToColor(color);
            Density = density;
        }
    }

    public class SkyColorTask : ITask {
        public readonly Color Color;

        public SkyColorTask(string color) {
            Color = StoryManager.StringToColor(color);
        }
    }

    public class FadeTask : ITask {
        public readonly bool FadeIn;
        public readonly float Duration;

        public FadeTask(bool fadeIn, float duration) {
            FadeIn = fadeIn;
            Duration = duration;
        }
    }

    public class SceneLoadTask : ITask {
        public readonly string SceneName;

        public SceneLoadTask(string sceneName) {
            SceneName = sceneName;
        }
    }
}