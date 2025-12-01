namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Entities/Multi Entity")]
    public class EntityMultiNode : EntityProviderNode {
        public EntitySO[] Entities;

        public override EntitySource GetSource() {
            if (Entities == null || Entities.Length == 0) return null;
            return EntitySource.Multiple(Entities);
        }
    }
}