using Ink.Runtime;
using KH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Pen {
    public class DungeonFunctionRegistrar : MonoBehaviour, IInkFunctionRegistrar {
        public void RegisterOn(Story story, TaskQueue queue) {
            story.BindExternalFunction("setMusic", (string music) => {
                queue.Enqueue(new MusicTask(music));
            });
            story.BindExternalFunction("setFog", (string color, float density) => {
                queue.Enqueue(new FogTask(color, density));
            });
            story.BindExternalFunction("setSky", (string color) => {
                queue.Enqueue(new SkyColorTask(color));
            });
            story.BindExternalFunction("fadeIn", (bool faded, float duration) => {
                queue.Enqueue(new FadeTask(faded, duration));
            });
            story.BindExternalFunction("showSplashText", (string text) => {
                queue.Enqueue(new SplashTextTask(text));
            });
            story.BindExternalFunction("goToTitle", () => {
                queue.Enqueue(new SceneLoadTask("MenuScene"));
            });
        }
    }
}