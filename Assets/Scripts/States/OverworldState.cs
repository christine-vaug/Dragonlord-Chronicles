using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateManagement {

    public class OverworldState : IGameState {

        List<GameObject> overworldGameObjects;

        /// <summary>
        /// Function called when the state is entered.
        /// Parameter 1: string denoting the scene to load.
        /// </summary>
        /// <param name="parameters"></param>
        /// 
        public void OnStateEnter(params object[] parameters) {
            // 1 - Push the scene loader to load the next scene
            GameManager.instance.PushState(GameStateType.SceneLoad, parameters);
            if (GlobalFlags.GetCurrentOverworldScene() == "Town")
                SoundManager.Instance.PlayMusic("Peaceful Village");
            else if (GlobalFlags.GetCurrentOverworldScene() == "Town - Interior")
                SoundManager.Instance.PlayMusic("RPG Simple Shop");
            else if (GlobalFlags.GetCurrentOverworldScene() == "Overworld" || GlobalFlags.GetCurrentOverworldScene() == "Overworld - West")
                SoundManager.Instance.PlayMusic("SNES RPG overworld loop II", 0.35f);
            else if (GlobalFlags.GetCurrentOverworldScene() == "Caves")
                SoundManager.Instance.PlayMusic("perces");
        }

        public void OnStateExit(params object[] parameters) {
            SoundManager.Instance.StopMusic();

        }

        public void OnStatePause() {


        }

        public void OnStateResume() {


        }

        public void OnStateUpdate(float dt) {



        }

    }
}

