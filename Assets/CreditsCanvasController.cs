using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsCanvasController : MonoBehaviour {

    RectTransform rect;

	// Use this for initialization
	void Start () {

        rect = GetComponent<RectTransform>();

	}
	
	// Update is called once per frame
	void Update () {

        rect.position = new Vector3(rect.position.x, rect.position.y + 45f * Time.deltaTime, rect.position.z);
	}
}
