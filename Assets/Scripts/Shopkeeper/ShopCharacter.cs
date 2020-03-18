using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCharacter : MonoBehaviour {

    public enum ShopType {
        GeneralGoods = 1,
        WeaponsArmor = 2,
    }

    public ShopKeeperData shopData;
    public ShopType type;

    void Awake () {
        


    }
}
