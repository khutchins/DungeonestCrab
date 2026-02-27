using Pomerandomian;
using System.Collections.Generic;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Modify Tags")]
    public class ActionTagNode : ActionProviderNode {
        public List<string> TagsToAdd = new List<string>();
        public List<string> TagsToRemove = new List<string>();
        public override ITileAction GetAction() => new TagAction(TagsToAdd, TagsToRemove);

        private class TagAction : ITileAction {
            readonly List<string> _add, _remove;
            public TagAction(List<string> add, List<string> remove) { _add = add; _remove = remove; }
            public void Apply(TileSpec spec, IRandom rand) {
                foreach (var t in _remove) spec.RemoveTag(t);
                foreach (var t in _add) spec.AddTag(t);
            }
        }
    }
}
