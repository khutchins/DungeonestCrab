using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyedList<T> : ScriptableObject {
    [System.Serializable]
    public struct Info {
        public string Key;
        public T Item;

        public bool IsValid() {
            return !string.IsNullOrEmpty(Key) && Item != null;
        }
    }

    [SerializeField] Info[] Infos;

    public Info InfoForKey(string name) {
        string normName = name.ToLower();
        return Infos.Where(x => x.Key.ToLower() == normName).FirstOrDefault();
    }

    public T KeyedItemForKey(string name) {
        Info info = InfoForKey(name);
        if (info.IsValid()) return info.Item;
        else return default;
    }

    public T GetOrWarn(string key, string type) {
        var item = KeyedItemForKey(key);
        if (item == null) {
            Debug.LogWarning($"{type} {key} does not exist!");
        }
        return item;
    }
}
