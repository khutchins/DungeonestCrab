using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Speakers")]
public class SpeakerInformation : ScriptableObject
{
    [System.Serializable]
    public class Info {
        public string Key;
        public Color NameColor;
        public Sprite Portrait;
        public float Pitch = 1 ;
    }

    [SerializeField] Info[] Speakers;

    public Info InfoForSpeaker(string name) {
        string normName = name.ToLower();
        return Speakers.Where(x => x.Key.ToLower() == normName).FirstOrDefault();
    }
}
