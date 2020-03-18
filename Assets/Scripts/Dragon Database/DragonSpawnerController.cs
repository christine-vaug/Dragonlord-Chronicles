using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSpawnerController : MonoBehaviour {

    public static DragonSpawnerController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<DragonSpawnerController>();
            }
            return instance;
        }
    }
    private static DragonSpawnerController instance;



    DragonSpawner[] spawners;

	// Use this for initialization
    void Awake () {

        instance = this;
    }

	void Start () {

        spawners = Object.FindObjectsOfType<DragonSpawner>();

        foreach (DragonSpawner sp in spawners) {

            GameObject go;
            if(sp.TrySpawnEnemy(out go)) {

            }
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
