using KH;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using System;

namespace DungeonestCrab.Dungeon {

	public class Entity : IEquatable<Entity> {
		public delegate void CodeOnInstantiate(TheDungeon dungeon, GameObject go, Entity entity, IRandom random);

		public readonly Vector2Int Tile;
		/// <summary>
		/// The index in which the entity was created as part of the adder. As a result, multiple entities can have the same index.
		/// </summary>
		public readonly int EntityIndex;
		public readonly EntitySpec Type;
		public readonly CodeOnInstantiate Code;
		public readonly float YAngle;
		public readonly float YOffset;
		public readonly string EntityID;

		public Entity(Vector2Int tile, int entityIndex, EntitySpec type, CodeOnInstantiate code = null, string entityID = null, float yAngle = 0, float yOffset = 0) {
			Tile = tile;
			EntityIndex = entityIndex;
			Type = type;
			Code = code;
			EntityID = entityID;
			YAngle = yAngle;
			YOffset = yOffset;
		}

		public Entity(Vector2Int tile, int entityIndex, EntitySO type, CodeOnInstantiate code = null, string entityID = null, float yAngle = 0, float yOffset = 0)
			: this(tile, entityIndex, type.Entity, code, entityID, yAngle, yOffset) {
		}

		public Entity(Vector2Int tile, int entityIndex, EntitySource.Pair pair, string entityID = null, float yAngle = 0, float yOffset = 0)
			: this(tile, entityIndex, pair.Entity, pair.Code, entityID, yAngle, yOffset == 0 ? pair.YOffset : yOffset) {
		}

		public override bool Equals(object obj) {
			if (obj == null || !this.GetType().Equals(obj.GetType())) return false;
			return Equals((Entity)obj);
		}

		public bool Equals(Entity other) {
			return other != null &&
				   EqualityComparer<Vector2Int>.Default.Equals(Tile, other.Tile) &&
				   EntityIndex == other.EntityIndex &&
				   EqualityComparer<EntitySpec>.Default.Equals(Type, other.Type) &&
				   YAngle == other.YAngle &&
				   YOffset == other.YOffset &&
				   EntityID == other.EntityID;
		}

		public override int GetHashCode() {
            return HashCode.Combine(Tile, EntityIndex, Type, YAngle, YOffset, EntityID);
        }
    }
}