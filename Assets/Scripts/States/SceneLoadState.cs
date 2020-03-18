using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace StateManagement {

    public class SceneLoadState : IGameState {

        string nextSceneName = "";

        public void OnStateEnter(params object[] parameters) {

            nextSceneName = (string)parameters[0];
            SceneManager.LoadScene(nextSceneName);
        }

        public void OnStateExit(params object[] parameters) {


        }

        public void OnStatePause() {


        }

        public void OnStateResume() {


        }

        public void OnStateUpdate(float dt) {

            if (SceneManager.GetActiveScene().name == nextSceneName) {
                GameManager.instance.PopState();
            }
        }
    }
}
