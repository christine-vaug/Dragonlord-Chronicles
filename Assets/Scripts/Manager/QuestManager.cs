using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using QuestSystem;

public class QuestManager : Manager {

    public Dictionary<int, Quest> questIDMap;


    public override void Awake() {
        
    }

    public override void Start() {

        
    }

    private void LoadAllQuests () {


    }

    public Quest GetQuest (int id) {

        Quest quest;
        if (questIDMap.TryGetValue(id, out quest)) {
            return quest;
        }

        Debug.LogError("Quest with ID: " + id + " does not exist!");
        return null;
    }

}
