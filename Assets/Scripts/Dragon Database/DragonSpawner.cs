using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSpawner : MonoBehaviour {



    [Tooltip("Bounds are centered at GameObject position")]
    public Vector2 size;

    [Tooltip("Place dragons that should spawn within the radius")]
    public GameObject dragonPrefab;

    [Tooltip("Chance of a dragon spawning in this region")]
    [Range(0f, 1f)]
    public float probability;

    void Start () {


    }

    public bool TrySpawnEnemy (out GameObject go) {

        if (probability >= Random.Range(0f, 1f)) {

            go = Instantiate(dragonPrefab);

            Vector2 extents = size / 2;

            float x = Random.Range(-extents.x, extents.x);
            float y = Random.Range(-extents.y, extents.y);

            go.transform.position = transform.position + new Vector3(x, y, -1f);
            return true;
        }


        go = null;
        return false;
    }



    void OnDrawGizmos() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 0f));
    }
}
