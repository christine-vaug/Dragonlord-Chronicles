using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class loads various things from the resources folder, so they can be used at runtime. 
/// </summary>
public class ResourcesManager : Manager {

    static Dictionary<string, GameObject> prefabGOMap;

    public override void Awake() {

        prefabGOMap = new Dictionary<string, GameObject>();

        LoadResources();   
    }

    public override void Start() {
        
    }


    private void LoadResources () {

        GameObject[] prefs = Resources.LoadAll<GameObject>("PreFabs");


        for (int i = 0; i < prefs.Length; i++) {
            prefabGOMap.Add(prefs[i].name, prefs[i]);
        }
    }


    /// <summary>
    /// Instantiate a prefab that was loaded from the resources folder
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Returns new instance of a gameObject if prefab exists. Returns a new gameObject otherwise. </returns>
    public static GameObject InstantiatePrefab (string name) {

        GameObject go;
        if (prefabGOMap.TryGetValue(name, out go)) return Object.Instantiate(go);

        Debug.LogError("Prefab named " + name + " does not exist");
        return new GameObject();
    }
}
