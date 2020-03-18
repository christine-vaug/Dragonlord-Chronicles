using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonlordChroniclesDatabase;

public class Dragon : Entity {

    public EntityData dragonData;

    // Use this for initialization
    void Start()
    {
        GlobalFlags.GetDragonFlag(out dragonData);

        if(dragonData != null)
            GetComponent<SpriteRenderer>().sprite = dragonData.battleSprite;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
