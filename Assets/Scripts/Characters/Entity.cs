using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonlordChroniclesDatabase;

public class Entity : MonoBehaviour {

    public ScriptableObject entityData;

    protected new SpriteRenderer renderer;
    protected new BoxCollider2D collider;
    protected new Rigidbody2D rigidbody;
    //protected InventorySystem.ItemList items;

    void Awake () {


    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
