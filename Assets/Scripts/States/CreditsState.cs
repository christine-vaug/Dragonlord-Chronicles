using System.Collections;
using System.Collections.Generic;

namespace StateManagement {


    public class CreditsState : IGameState {

        public void OnStateEnter(params object[] parameters) {

            UnityEngine.SceneManagement.SceneManager.LoadScene(8);

        }

        public void OnStateExit(params object[] parameters) {

        }

        public void OnStatePause() {

        }

        public void OnStateResume() {

        }

        public void OnStateUpdate(float dt) {

        }
    }

}
