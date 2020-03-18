using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Warps the player to a location within the same scene
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SceneWarp : MonoBehaviour {

    public Transform warpTarget;

    void OnTriggerEnter2D (Collider2D coll) {

        coll.transform.position = warpTarget.position;
    }
}
