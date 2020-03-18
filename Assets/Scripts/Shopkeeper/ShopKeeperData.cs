using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Shopkeeper", menuName = "Scriptable Objects/Shopkeeper")]
public class ShopKeeperData : ScriptableObject {
    
    public int Gold;
    public InventorySystem.Inventory items;
    public bool needsInitialized = true;




}
