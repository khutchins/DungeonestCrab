using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
	/// <summary>
	/// A source of entities to be passed into Alterers to cut down on the 
	/// number of overrides they need to do while maintaining flexibility.
	/// </summary>
    public class EntitySource {

		private readonly Pair[] _pairs;

		public EntitySource(params Pair[] pairs) {
			_pairs = pairs;
		}

		/// <summary>
		/// Returns an entity from the source.
		/// </summary>
		/// <param name="random"></param>
		/// <returns></returns>
        public virtual Pair GetPair(IRandom random) {
			if (_pairs == null || _pairs.Length == 0) return null;
			return random.From(_pairs);
		}

		/// <summary>
		/// Returns an entity source that returns a single entity.
		/// </summary>
		/// <param name="entity">The entity to return</param>
		/// <param name="code">The code on instantiate</param>
		/// <returns></returns>
		public static EntitySource Single(EntitySO entity, Entity.CodeOnInstantiate code = null) {
			return new EntitySource(new Pair(entity, code));
		}

		/// <summary>
		/// Returns an entity source that returns a single entity.
		/// </summary>
		/// <param name="entity">The entity to return</param>
		/// <param name="code">The code on instantiate</param>
		/// <returns></returns>
		public static EntitySource Single(EntitySpec entity, Entity.CodeOnInstantiate code = null) {
			return new EntitySource(new Pair(entity, code));
		}

		/// <summary>
		/// Returns an entity source that returns any of the entities passed in, chosen randomly.
		/// </summary>
		/// <returns></returns>
		public static EntitySource Multiple(params EntitySO[] entities) {
			Builder builder = new Builder();
			foreach(var entity in entities) {
				builder.AddEntity(entity);
			}
			return builder.Build();
		}

		/// <summary>
		/// Returns an entity source that returns any of the entities passed in, chosen randomly.
		/// </summary>
		/// <returns></returns>
		public static EntitySource Multiple(params EntitySpec[] entities) {
			Builder builder = new Builder();
			foreach (var entity in entities) {
				builder.AddEntity(entity);
			}
			return builder.Build();
		}

		public class Pair {
			public readonly EntitySpec Entity;
			public readonly Entity.CodeOnInstantiate Code;

			public Pair(EntitySO entity, Entity.CodeOnInstantiate code) {
				Entity = entity.Entity;
				Code = code;
			}

			public Pair(EntitySpec entity, Entity.CodeOnInstantiate code) {
				Entity = entity;
				Code = code;
			}
		}

		public class Builder {
			private List<Pair> _pairs = new List<Pair>();

			public Builder() {

			}

			public Builder AddEntity(EntitySO entity, Entity.CodeOnInstantiate code = null) {
				_pairs.Add(new Pair(entity, code));
				return this;
			}

			public Builder AddEntity(EntitySpec entity, Entity.CodeOnInstantiate code = null) {
				_pairs.Add(new Pair(entity, code));
				return this;
			}

			public EntitySource Build() {
				return new EntitySource(_pairs.ToArray());
			}
		}
	}
}