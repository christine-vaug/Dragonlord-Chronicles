using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDemo : MonoBehaviour {

    public class Thing {

        public System.Action OnThingDone;
    }

            
	// Use this for initialization
	void Start () {

        Thing t1 = new Thing();
        Thing t2 = new Thing();

        t1.OnThingDone = PrintA;
        t2.OnThingDone = PrintB;

        t1.OnThingDone();
        t2.OnThingDone();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void PrintA () {

        Debug.Log("A");
    }

    void PrintB () {

        Debug.Log("B");
    }
}
