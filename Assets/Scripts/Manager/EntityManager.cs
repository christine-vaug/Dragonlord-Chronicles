using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages different entities. Not yet implemented.
/// </summary>
public class EntityManager : Manager {

    static Dictionary<string, ScriptableObject> scriptableObjects;
    static List<Entity> entities;

    public override void Awake() {

        entities = new List<Entity>();
        scriptableObjects = new Dictionary<string, ScriptableObject>();

        LoadAllScriptableObjects();
        
    }

    public override void Start() {


    }

    public static void CreateEntityOfType (string name, Vector3 position) {

        GameObject entityGO = ResourcesManager.InstantiatePrefab("Entity");
        entityGO.transform.position = position;

        Entity e = entityGO.GetComponent<Entity>();
        e.entityData = scriptableObjects[name];

        entities.Add(e);
    }

    private void LoadAllScriptableObjects () {

        ScriptableObject[] objects = Resources.LoadAll<ScriptableObject>("ScriptableObjects");

        foreach (ScriptableObject so in objects) {
            scriptableObjects.Add(so.name, so);
        }

    }

    public void DestroyEntity () {


    }
}
