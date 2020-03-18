using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

namespace QuestSystem {

    public enum Status {
        Inactive,
        Active,
        Complete,
        Failed,
    }
    
    public class Quest {

        public string Name;
        public string Description;
        public Status status;

        public Dictionary<int, QuestStage> stages;
        public QuestStage currentStage;

        public Reward reward;

        public void ActivateQuest () {

            status = Status.Active;
            reward = null; //Rewards should differ based on quest outcome...
        }

        public void AdvanceStage (int next) {

            if (stages.TryGetValue(next, out currentStage)) {

                if (currentStage.failsQuest) {
                    status = Status.Failed;
                }
                if (currentStage.succeedsQuest) {
                    status = Status.Complete;
                }
            }
        }
    }


}
