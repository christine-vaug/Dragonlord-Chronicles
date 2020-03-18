using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

namespace DragonlordChroniclesDatabase {


    [CreateAssetMenu(fileName = "New Player Container", menuName = "Scriptable Objects/Player")]
    public class PlayerData : EntityData {
            
        public List<DragonData> dragons;
        public List<Quest> discoveredQuest;
    }
}
