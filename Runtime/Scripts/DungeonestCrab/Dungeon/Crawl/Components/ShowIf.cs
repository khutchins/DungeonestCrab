using DungeonestCrab.Dungeon.Crawl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIf : DungeonBoolInkListener {

    private DungeonInteractable _interact;
    private bool _wasShowing = true;

    private void Awake() {
        _interact = GetComponent<DungeonInteractable>();
    }

    protected override void OnStateChange(bool show) {
        if (show) {
            if (_interact != null) _interact.UnKill();
            else if (!_wasShowing) {
                Vector3 transform = this.transform.position;
                transform.y += DungeonInteractable.KILL_OFFSET;
                this.transform.position = transform;
            }
        } else {
            if (_interact != null) _interact.Kill();
            // We can't disable the object and still get notifications, so cheat it a bit.
            else if (_wasShowing) {
                Vector3 transform = this.transform.position;
                transform.y -= DungeonInteractable.KILL_OFFSET;
                this.transform.position = transform;
            }
        }
        _wasShowing = show;
    }
}
