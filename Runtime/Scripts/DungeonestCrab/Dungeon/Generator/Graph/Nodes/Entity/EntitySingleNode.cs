namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Entities/Single")]
    public class EntitySingleNode : EntityProviderNode {
        public EntitySO Entity;

        public override EntitySource GetSource() {
            if (Entity == null) return null;
            return EntitySource.Single(Entity);
        }
    }
}