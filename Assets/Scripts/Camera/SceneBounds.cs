using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBounds : MonoBehaviour {

    [Tooltip("Bounds are centered at GameObject position")]
    public Vector2 size;

    public Vector2 GetCenter () {

        return new Vector2(transform.position.x, transform.position.y);
    }

    public Vector2 GetExtents () {

        return new Vector2(size.x / 2f, size.y / 2f);
    }

    void OnDrawGizmos () {

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 0f));
    }
}
