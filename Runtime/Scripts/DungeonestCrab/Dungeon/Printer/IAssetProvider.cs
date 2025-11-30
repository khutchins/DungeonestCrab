using Pomerandomian;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    enum ProviderType {
        Single,
        Random,
        RandomWithShares
    }

    [InlineEditor]
    public class IAssetProvider<T> : ScriptableObject {

        [SerializeField] ProviderType ProviderType;

        [ShowIf("ProviderType", ProviderType.Single)]
        [SerializeField] T Object;
        [ShowIf("ProviderType", ProviderType.Random)]
        [SerializeField] T[] Objects;
        [ShowIf("ProviderType", ProviderType.RandomWithShares)]
        [SerializeField] ObjectOdds<T>[] ObjectAndShares;

        public T GetAsset(IRandom random) {
            switch (ProviderType) {
                case ProviderType.Single:
                    return Object;
                case ProviderType.Random:
                    return random.From(Objects);
                case ProviderType.RandomWithShares:
                    return random.FromWithOdds(ObjectAndShares);
            }
            return default;
        }

        private void OnValidate() {
            switch (ProviderType) {
                case ProviderType.Single:
                    Objects = default;
                    ObjectAndShares = default;
                    break;
                case ProviderType.Random:
                    Object = default;
                    ObjectAndShares = default;
                    break;
                case ProviderType.RandomWithShares:
                    Object = default;
                    Objects = default;
                    break;
            }
        }
    }
}