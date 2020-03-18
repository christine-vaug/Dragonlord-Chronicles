using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateManagement {

    public class BattleState : IGameState {

        //public ScriptableObject PlayerData = Resources.Load<ScriptableObject>("ScriptableObjects/Player");
        //public ScriptableObject DragonData = Resources.Load<ScriptableObject>("ScriptableObjects/Dragon");
        //public ScriptableObject EnemyData = Resources.Load<ScriptableObject>("ScriptableObjects/ExampleEnemy");

        public void OnStateEnter(params object[] parameters) {

            SceneManager.LoadScene(GlobalFlags.GetCurrentBattleScene());
            if (GlobalFlags.GetBossFlag() == false)
            {
                SoundManager.Instance.PlayMusic("SuperHero_original", 0.55f);
            }
            else
            {
                SoundManager.Instance.PlayMusic("Boss Battle", 0.55f);
            }

        }

        public void OnStateExit(params object[] parameters) {

            SoundManager.Instance.StopMusic();
            SceneManager.LoadScene(GlobalFlags.GetCurrentOverworldScene());
            if (GlobalFlags.GetCurrentOverworldScene() == "Town")
                SoundManager.Instance.PlayMusic("Peaceful Village");
            else if (GlobalFlags.GetCurrentOverworldScene() == "Town - Interior")
                SoundManager.Instance.PlayMusic("RPG Simple Shop");
            else if (GlobalFlags.GetCurrentOverworldScene() == "Overworld" || GlobalFlags.GetCurrentOverworldScene() == "Overworld - West")
                SoundManager.Instance.PlayMusic("SNES RPG overworld loop II", 0.35f);
            else if (GlobalFlags.GetCurrentOverworldScene() == "Caves")
                SoundManager.Instance.PlayMusic("perces");
        }

        public void OnStatePause() {

        }

        public void OnStateResume() {

        }

        public void OnStateUpdate(float dt) {


        }

        
    }


}

