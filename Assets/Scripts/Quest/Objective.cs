using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem {

    public enum ObjectiveType {

        ItemsInInventory,
        Money,
        EnemiesKilled,
    }

    public class Objective {

        public ObjectiveType type;
        public bool isComplete;
    }

}
