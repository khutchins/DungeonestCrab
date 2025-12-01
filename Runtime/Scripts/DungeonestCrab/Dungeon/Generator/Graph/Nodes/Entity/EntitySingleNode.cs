namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Entities/Single Entity")]
    public class EntitySingleNode : EntityProviderNode {
        public EntitySO Entity;

        public override EntitySource GetSource() {
            if (Entity == null) return null;
            return EntitySource.Single(Entity);
        }
    }
}