using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem {

    public enum PrerequisiteType {

        Level,
        Experience,
        CompleteQuests,
    }

    public class Prerequisite {

        public PrerequisiteType Type { get; private set; }

        public int levelCount;
        
    }

}