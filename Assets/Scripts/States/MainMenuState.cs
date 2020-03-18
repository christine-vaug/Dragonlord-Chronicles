using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace StateManagement {

    public class MainMenuState : IGameState {

        GameObject menuGO;

        Button newGameButton;
        Button loadGameButton;

        public void OnStateEnter(params object[] parameters) {

            if (menuGO == null) {
                
                menuGO = ResourcesManager.InstantiatePrefab("MainMenuCanvas");
                MonoBehaviour.DontDestroyOnLoad(menuGO);
            }
            SoundManager.Instance.PlayMusic("Kings And Dragons", 0.75f);
        }

        public void OnStateExit(params object[] parameters) {

            SoundManager.Instance.StopMusic();
            Application.Quit();
        }

        public void OnStatePause() {

            Object.Destroy(menuGO);

        }

        public void OnStateResume() {

            if (GameManager.instance != null) {
                Object.Destroy(GameManager.instance.gameObject);
            }
            if (SoundManager.Instance != null) {
                Object.Destroy(SoundManager.Instance.gameObject);
            }

            if (ShopkeeperCanvasController.Instance != null) {
                Object.Destroy(ShopkeeperCanvasController.Instance.gameObject);
            }


            menuGO = ResourcesManager.InstantiatePrefab("MainMenuCanvas");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            menuGO.SetActive(true);
        }

        public void OnStateUpdate(float dt) {


        }
    }
}

