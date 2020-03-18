using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Warps the player to a location in a different scene
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MapWarp : MonoBehaviour {

    public string mapName;
    public Vector2 location;
    
    void OnTriggerEnter2D(Collider2D coll) {

        if (coll.tag == "Player") {

            //music should not stop or change if moving from overworld to overworld - west or vice versa
            if(GlobalFlags.GetCurrentOverworldScene() != "Overworld" || mapName != "Overworld - West")
            {
                if (GlobalFlags.GetCurrentOverworldScene() != "Overworld - West" || mapName != "Overworld")
                    SoundManager.Instance.StopMusic();
            }

            GlobalFlags.SetCurrentOverworldScene(mapName);
            GlobalFlags.SetPlayerPosition(location);

            SceneManager.LoadScene(GlobalFlags.GetCurrentOverworldScene());

            if (GlobalFlags.GetCurrentOverworldScene() != "Overworld" || mapName != "Overworld - West")
            {
                if (GlobalFlags.GetCurrentOverworldScene() != "Overworld - West" || mapName != "Overworld")
                {
                    if (mapName == "Town")
                        SoundManager.Instance.PlayMusic("Peaceful Village");
                    else if (mapName == "Town - Interior")
                        SoundManager.Instance.PlayMusic("RPG Simple Shop");
                    else if (mapName == "Overworld" || mapName == "Overworld - West")
                        SoundManager.Instance.PlayMusic("SNES RPG overworld loop II", 0.35f);
                    else if (mapName == "Caves")
                        SoundManager.Instance.PlayMusic("perces");
                }
            }
        }
    }
}