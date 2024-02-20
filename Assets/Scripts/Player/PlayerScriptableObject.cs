using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player{
    [Serializable] class UnlockedAbilities {
        public bool laser;
        public bool shield;
        public bool dash;
        public bool minionSpawn;
    }

    [CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObjects/Player/PlayerScriptableObject", order = 0)]
    public class PlayerScriptableObject : ScriptableObject
    {
        public GameObject laser;
        public GameObject shield;
        public GameObject minion;
        [SerializeField] private UnlockedAbilities unlockedAbilities;

        public Dictionary<string, GameObject> GetAbilities() {
            Dictionary<string, GameObject> abilities = new Dictionary<string, GameObject>();
            if (unlockedAbilities.laser) {
                abilities.Add("laser", laser);
            }
            if (unlockedAbilities.shield) {
                abilities.Add("shield", shield);
            }
            if (unlockedAbilities.minionSpawn) {
                abilities.Add("minion", minion);
            }
            return abilities;
        }
    }
}