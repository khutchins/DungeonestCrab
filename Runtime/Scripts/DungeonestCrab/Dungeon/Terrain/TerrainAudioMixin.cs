using KH.Audio;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace DungeonestCrab.Dungeon {

    [Serializable]
    public class TerrainAudioMixin : TerrainMixin {
        [Title("Audio Settings")]
        [SerializeField] AudioEvent FootstepSound;

        public TerrainAudioMixin(AudioEvent footstepSound) {
            FootstepSound = footstepSound;
        }

        public virtual AudioEvent GetFootstepSound() { 
            return FootstepSound;
        }
    }
}