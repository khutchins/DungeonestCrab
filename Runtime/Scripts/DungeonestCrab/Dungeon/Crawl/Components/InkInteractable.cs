using DungeonestCrab.Dungeon.Crawl;
using DungeonestCrab.Dungeon.Pen;
using Ink.Runtime;
using KH.Texts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    public class InkInteractable : DungeonInteractable {
        public string InkPassage;
        public bool HideAfterInteraction;

        protected override void OnStart() {
            CheckHide();
        }

        protected override void DoAction() {
            StartCoroutine(RunInkPassage());
        }

        IEnumerator RunInkPassage() {
            yield return DungeonReader.INSTANCE.PerformInkActionCoroutine(InkPassage);
            CheckHide();
            ActionFinished(null);
        }

        void CheckHide() {
            if (HideAfterInteraction && InkStateManager.INSTANCE.Manager.VisitCount(InkPassage) > 0) {
                Kill();
            }
        }
    }
}