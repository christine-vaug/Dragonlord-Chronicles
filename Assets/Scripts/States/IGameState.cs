using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateManagement {


    public interface IGameState {

        void OnStateEnter(params object[] parameters);
        void OnStatePause();
        void OnStateUpdate(float dt);
        void OnStateResume();
        void OnStateExit(params object[] parameters);
    }
}
