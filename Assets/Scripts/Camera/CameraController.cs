using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private SceneBounds bounds;
    private Vector2 boundsCenter, boundsExtents;

    private Camera cam;
    public Transform target;


	// Use this for initialization
    void Awake () {

        cam = GetComponent<Camera>();

        bounds = Object.FindObjectOfType<SceneBounds>();
        if (bounds != null) {
            boundsCenter = bounds.GetCenter();
            boundsExtents = bounds.GetExtents();

            float height = cam.orthographicSize;
            float width = height * cam.aspect;

            Vector3 target = this.target.position;
            target.z = -50f;

            if (target.y - height < (boundsCenter.y - boundsExtents.y)) {
                target.y = (boundsCenter.y - boundsExtents.y) + height;
            }

            if (target.y + height > (boundsCenter.y + boundsExtents.y)) {
                target.y = (boundsCenter.y + boundsExtents.y) - height;
            }

            if (target.x - width < (boundsCenter.x - boundsExtents.x)) {
                target.x = (boundsCenter.x - boundsExtents.x) + width;
            }

            if (target.x + width > (boundsCenter.x + boundsExtents.x)) {
                target.x = (boundsCenter.x + boundsExtents.x) - width;
            }

            transform.position = target;

        }
    }

	void Start () {


    }
	
	// Update is called once per frame
	void Update () {

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        Vector3 target = this.target.position;
        target.z = -50f;

        //Is the camera restricted?
        if (bounds != null) {

            if (target.y - height < (boundsCenter.y - boundsExtents.y)) {
                target.y = (boundsCenter.y - boundsExtents.y) + height;
            }

            if (target.y + height > (boundsCenter.y + boundsExtents.y)) {
                target.y = (boundsCenter.y + boundsExtents.y) - height;
            }

            if (target.x - width < (boundsCenter.x - boundsExtents.x)) {
                target.x = (boundsCenter.x - boundsExtents.x) + width;
            }

            if (target.x + width > (boundsCenter.x + boundsExtents.x)) {
                target.x = (boundsCenter.x + boundsExtents.x) - width;
            }

        }


        transform.position = target;
	}
}
