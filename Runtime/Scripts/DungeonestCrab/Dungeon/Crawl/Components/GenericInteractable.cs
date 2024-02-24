using System.Collections;

namespace DungeonestCrab.Dungeon.Crawl {
    public class GenericInteractable : DungeonInteractable {

        public System.Action Action;

        protected override void DoAction() {
            StartCoroutine(DoActionCoroutine());
        }

        IEnumerator DoActionCoroutine() {
            Action?.Invoke();
            yield return null;
            ActionFinished(null);
        }
    }
}