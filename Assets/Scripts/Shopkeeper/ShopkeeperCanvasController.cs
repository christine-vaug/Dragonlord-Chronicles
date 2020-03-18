using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using DragonlordChroniclesDatabase;
using UnityEngine.UI;

public class ShopkeeperCanvasController : MonoBehaviour {

    private static ShopkeeperCanvasController _instance;
    public static ShopkeeperCanvasController Instance {
        get {
            if (_instance != null) return _instance;

            _instance = Object.FindObjectOfType<ShopkeeperCanvasController>();
            if (_instance != null) return _instance;

            //Debug.LogError("Scene does not have a ShopkeeperCanvasController");
            return null;
        }
    }

    public Text playerText;
    public Text shopText;
    public Text costText;

    public GameObject buttonPrefab;

    public Transform playerItemWindow;
    public Transform shopItemWindow;
    public Transform transactionDetailsWindow;


    ShopKeeperData shopData;
    Inventory playerInventory;
    EntityData playerData;

    int TransactionCost;

    Inventory itemsToBuy;
    Inventory itemsToSell;


    void Awake () {

        _instance = this;
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }

    public void Activate (ShopKeeperData shopData, Inventory playerInventory, EntityData playerData) {

        if (shopData == null) {
            Debug.LogError("Shop Data cannot be null");
            return;
        }

        if (playerInventory == null) {
            Debug.LogError("Player Inventory  cannot be null");
            return;
        }
        if (playerData == null) {
            Debug.LogError("Player Data cannot be null");
            return;
        }

        TransactionCost = 0;

        itemsToBuy = ScriptableObject.CreateInstance<Inventory>();
        itemsToSell = ScriptableObject.CreateInstance<Inventory>();
        
        gameObject.SetActive(true);

        this.shopData = shopData;
        this.playerInventory = playerInventory;
        this.playerData = playerData;

        playerText.text = "Player Inventory\n" + playerData.Gold + " Gold";
        shopText.text = "Shop Inventory\n" + shopData.Gold + " Gold";
        
        List<Tuple> playerItems = playerInventory.GetInventorySpace();
        int count = playerItems.Count;

        for (int i = 0; i < count; i++) {
            GameObject go = Instantiate(buttonPrefab);
            go.transform.SetParent( playerItemWindow );

            Button b = go.GetComponent<Button>();
            b.onClick.AddListener(() => OnSellItem(b));

            b.name = playerItems[i].ItemName + " | " + playerItems[i].Count;
            b.GetComponentInChildren<Text>().text = b.name;
        }

        List<Tuple> storeItems = shopData.items.GetInventorySpace();
        count = storeItems.Count;
        for (int i = 0; i < count; i++) {

            GameObject go = Instantiate(buttonPrefab);
            go.transform.SetParent( shopItemWindow );

            Button b = go.GetComponent<Button>();
            b.onClick.AddListener(() => OnPurchaseItem(b));

            b.name = storeItems[i].ItemName + " | " + storeItems[i].Count;
            b.GetComponentInChildren<Text>().text = b.name;
        }
    }

    void Update () {

        //If UI causes poor performance, this is probably why...
        if (itemsToBuy.GetInventorySpace().Count != 0 || itemsToSell.GetInventorySpace().Count != 0) {

            if (TransactionCost >= 0)
                costText.text = "Purchase: (" + TransactionCost.ToString() + ")";
            else if (TransactionCost < 0)
                costText.text = "Sell: (" + Mathf.Abs(TransactionCost).ToString() + ")";
            
        }
        else
            costText.text = "Exit";

    }


    public void FinishTransaction () {

        if (itemsToBuy.GetInventorySpace().Count == 0 && itemsToSell.GetInventorySpace().Count == 0) {
            OnCancelTransaction();
            return;
        }

        if (Internal_OnFinishTransaction() == true)  {

            if (itemsToBuy.GetInventorySpace().Count == 0 && itemsToSell.GetInventorySpace().Count == 0) {

                gameObject.SetActive(false);

                Destroy(itemsToBuy);
                Destroy(itemsToSell);

                ClearAllButtons();
            } else {

                ClearAllButtons();
                Activate(shopData, playerInventory, playerData);
            }
        }
        //Player should not be able to finish if it's too expensive...
    }

    public bool Internal_OnFinishTransaction () {




        if (TransactionCost > playerData.Gold) {
            return false;
        }

        if (TransactionCost < 0 && TransactionCost > shopData.Gold) {
            TransactionCost = shopData.Gold;
        }


        playerData.Gold -= TransactionCost;
        shopData.Gold += TransactionCost;

        if (shopData.Gold < 0) shopData.Gold = 0;

        List<Tuple> buy = itemsToBuy.GetInventorySpace();
        foreach (Tuple t in buy) {

            for (int i = 0; i < t.Count; i++) {
                playerInventory.Insert(t.ItemName);
                shopData.items.Remove(t.ItemName);
            }
        }

        List<Tuple> sell = itemsToSell.GetInventorySpace();
        foreach (Tuple t in sell) {

            for (int i = 0; i < t.Count; i++) {
                playerInventory.Remove(t.ItemName);
                shopData.items.Insert(t.ItemName);
            }
        }

        return true;
    }

    public void OnCancelTransaction () {

        TransactionCost = 0;
        Destroy(itemsToBuy);
        Destroy(itemsToSell);

        gameObject.SetActive(false);
        ClearAllButtons();
        
    }

    public bool IsFinished () {

        //Not active -> is finished
        return gameObject.activeInHierarchy == false;
    }

    private void ClearAllButtons () {

        int count = playerItemWindow.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(playerItemWindow.GetChild(i).gameObject);
        }
        count = shopItemWindow.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(shopItemWindow.GetChild(i).gameObject);
        }
        count = transactionDetailsWindow.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(transactionDetailsWindow.GetChild(i).gameObject);
        }

    }

    private void UpdateTransactionDetails () {

        int count = transactionDetailsWindow.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(transactionDetailsWindow.GetChild(i).gameObject);
        }

        List<Tuple> buy = itemsToBuy.GetInventorySpace();
        count = buy.Count;
        for (int i = 0; i < count; i++) {

            GameObject go = Instantiate(buttonPrefab);
            go.transform.SetParent(transactionDetailsWindow);

            Button b = go.GetComponent<Button>();
            b.onClick.AddListener(() => OnCancelItem(b));

            b.name = "(B) " + buy[i].ItemName + " | " + buy[i].Count;
            b.GetComponentInChildren<Text>().text = b.name;
        }

        List<Tuple> sell = itemsToSell.GetInventorySpace();
        count = sell.Count;
        for (int i = 0; i < count; i++) {

            GameObject go = Instantiate(buttonPrefab);
            go.transform.SetParent(transactionDetailsWindow);

            Button b = go.GetComponent<Button>();
            b.onClick.AddListener(() => OnCancelItem(b));

            b.name = "(S) " + sell[i].ItemName + " | " + sell[i].Count;
            b.GetComponentInChildren<Text>().text = b.name;
        }
    }


    private void OnSellItem (Button b) {

        string[] split = b.name.Split(' ');
        int maxItems = 0;

        string itName = "";

        for (int i = 0; i < split.Length - 2; i++) {
            itName += split[i] + (i < split.Length - 3 ? " " : "");
        }

        List<Tuple> playerInventorySpace = playerInventory.GetInventorySpace();

        for (int i = 0; i < playerInventorySpace.Count; i++) {


            if (playerInventorySpace[i].ItemName == itName) {
                maxItems += playerInventorySpace[i].Count;
            }
        }



        List<Tuple> sell = itemsToSell.GetInventorySpace();
        int count = sell.Count;

        bool canInsert = true;
        int numInTransaction = 0;

        for (int i = 0; i < sell.Count; i++) {

            if (sell[i].ItemName == itName) {
                numInTransaction += sell[i].Count;
            }
            if (numInTransaction == maxItems) {
                canInsert = false;
            }
        }

        if (canInsert) {
            itemsToSell.Insert(itName, 1);
            TransactionCost -= GetItemWorth(itName, selling: true);
            UpdateTransactionDetails();

        }





    }

    private void OnPurchaseItem (Button b) {

        string[] split = b.name.Split(' ');
        int maxItems = 0;

        string itName = "";

        for (int i = 0; i < split.Length - 2; i++) {
            itName += split[i] + (i < split.Length - 3 ? " " : "");
        }

        List<Tuple> shopInventorySpace = shopData.items.GetInventorySpace();

        for (int i = 0; i < shopInventorySpace.Count; i++) {
            if (shopInventorySpace[i].ItemName == itName) {
                maxItems += shopInventorySpace[i].Count;
            }
        }

        List<Tuple> buy = itemsToBuy.GetInventorySpace();
        int count = buy.Count;

        bool canInsert = true;
        int numInTransaction = 0;

        for (int i = 0; i < buy.Count; i++) {

            if (buy[i].ItemName == itName) {
                numInTransaction += buy[i].Count;
            }
            if (numInTransaction == maxItems) {
                canInsert = false;
            }
        }

        if (canInsert) {
            itemsToBuy.Insert(itName, 1);
            TransactionCost += GetItemWorth(itName);
            UpdateTransactionDetails();

        }
        
    }

    private void OnCancelItem (Button b) {

        string name = b.name;
        string[] split = name.Split(' ');


        string itName = "";

        for (int i = 1; i < split.Length - 2; i++) {
            itName += split[i] + (i < split.Length - 3 ? " " : "");
        }


        if (name.Contains("(S) ")) {
            itemsToSell.Remove(itName);
            TransactionCost += GetItemWorth(itName, true);
        }
        if (name.Contains("(B) ")) {
            itemsToBuy.Remove(itName);
            TransactionCost -= GetItemWorth(itName);
        }

        UpdateTransactionDetails();
    }

    int GetItemWorth (string name, bool selling = false) {
        


        if (name == "Rusty Sword") return selling == false ? 100 : 50;
        if (name == "Sword") return selling == false? 500 : 250;
        if (name == "Knight Sword") return selling == false ? 1000 : 500;
        if (name == "Axe") return selling == false ? 100 : 50;
        if (name == "Great Axe") return selling == false ? 400 : 200;
        if (name == "Lance") return selling == false ? 1500 : 750;
        if (name == "Boss Killer Sword") return selling == false ? 0 : 0;

        if (name == "Apple") return selling == false ? 5 : 3;
        if (name == "Potion") return selling == false ? 50 : 20;
        if (name == "Mushroom") return selling == false ? 5 : 3;
        if (name == "Full Restore") return selling == false ? 100 : 50;
        if (name == "Lesser Potion") return selling == false ? 30 : 10;
        if (name == "Greater Potion") return selling == false ? 70 : 30;

        if (name == "Helmet") return selling == false ? 100 : 50;
        if (name == "Steel Helmet") return selling == false ? 300 : 150;
        if (name == "Platinum Helmet") return selling == false ? 500 : 250;

        if (name == "Chest Plate") return selling == false ? 100 : 50;
        if (name == "Steel Chest Plate") return selling == false ? 400 : 250;
        if (name == "Platinum Chest Plate") return selling == false ? 800 : 450;

        if (name == "Gloves") return selling == false ? 100 : 50;
        if (name == "Steel Gloves") return selling == false ? 300 : 150;
        if (name == "Platinum Gloves") return selling == false ? 500 : 250;

        if (name == "Boots") return selling == false ? 100 : 50;
        if (name == "Steel Boots") return selling == false ? 300 : 150;
        if (name == "Platinum Boots") return selling == false ? 500 : 250;

        Debug.LogWarning("Item named: " + name + " does not have a value!");
        return 0;
    }
}
