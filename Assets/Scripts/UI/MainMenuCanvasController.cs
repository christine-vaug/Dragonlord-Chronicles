using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvasController : MonoBehaviour {
    
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnNewGamePressed () {

        SerializationManager.CreateNewSaveFile();


        ShopKeeperData weaponsShop = Resources.Load<ShopKeeperData>("ScriptableObjects/Shops/ArmorShop");

        weaponsShop.Gold = 2000;
        weaponsShop.items.ResetInventory(player: false);
        weaponsShop.items.Insert("Axe", 1);
        weaponsShop.items.Insert("Helmet", 1);
        weaponsShop.items.Insert("Chest Plate", 1);
        weaponsShop.items.Insert("Gloves", 1);
        weaponsShop.items.Insert("Boots", 1);

        weaponsShop.items.Insert("Sword", 1);
        weaponsShop.items.Insert("Great Axe", 1);
        weaponsShop.items.Insert("Steel Helmet", 1);
        weaponsShop.items.Insert("Steel Chest Plate", 1);
        weaponsShop.items.Insert("Steel Gloves", 1);
        weaponsShop.items.Insert("Steel Boots", 1);

        weaponsShop.items.Insert("Knight Sword", 1);
        weaponsShop.items.Insert("Lance", 1);
        weaponsShop.items.Insert("Platinum Helmet", 1);
        weaponsShop.items.Insert("Platinum Chest Plate", 1);
        weaponsShop.items.Insert("Platinum Gloves", 1);
        weaponsShop.items.Insert("Platinum Boots", 1);

        ShopKeeperData goodsShop = Resources.Load<ShopKeeperData>("ScriptableObjects/Shops/Goods Shop");
        goodsShop.Gold = 2000;
        goodsShop.items.ResetInventory(player: false);
        goodsShop.items.Insert("Apple", 15);
        goodsShop.items.Insert("Lesser Potion", 5);
        goodsShop.items.Insert("Potion", 5);
        goodsShop.items.Insert("Greater Potion", 5);

        GlobalFlags.SetCurrentOverworldScene("Town");
        GlobalFlags.SetPlayerPosition(new Vector2(0.5f, -11f));
        GameManager.instance.PushState(StateManagement.GameStateType.Overworld, "Town");

        GlobalFlags.SetFirstTimeFlag(false);
    }

    public void OnLoadGamePressed () {


        if (SerializationManager.SaveExists() == false) {

            OnNewGamePressed();
            return;
        }

        Debug.Log("IS THIS THE FIRST TIME???");

        if (GlobalFlags.GetFirstTimeFlag() == true) {

            Debug.Log("YES!");

            DragonlordChroniclesDatabase.EntityData player = Resources.Load<DragonlordChroniclesDatabase.EntityData>("ScriptableObjects/Player");
            string[] lines;
            if (SerializationManager.GetTextFromFile("PlayerData.txt", out lines)) {

                player.Level = int.Parse( lines[0] );
                player.Experience = int.Parse(lines[1]);
                player.CurrentHealth = player.MaxHealth = int.Parse(lines[3]);
                player.Offense = int.Parse(lines[4]);
                player.Defense = int.Parse(lines[5]);
                player.Speed = int.Parse(lines[6]);
            }

            for (int i = 0; i < 4; i++) {

                if (SerializationManager.GetTextFromFile("DragonData_" + i.ToString() + ".txt", out lines)) {

                    player.DragonList[i].DisplayName = lines[0];

                    Debug.Log(lines[0]);

                    if (lines[0] != "")
                        player.DragonList[i].battleSprite = Resources.Load<DragonlordChroniclesDatabase.EntityData>("ScriptableObjects/Dragons/" + lines[0]).battleSprite;
                    else {
                        player.DragonList[i].battleSprite = null;
                        continue;
                    }

                    Debug.Log(lines[4]);

                    player.DragonList[i].Level = int.Parse(lines[1]);
                    player.DragonList[i].Experience = float.Parse(lines[2]);
                    player.DragonList[i].MaxHealth = player.DragonList[i].CurrentHealth = int.Parse(lines[4]);

                    player.DragonList[i].MaxMana = float.Parse(lines[6]);
                    player.DragonList[i].Magic = float.Parse(lines[6]);
                    player.DragonList[i].Speed = float.Parse(lines[7]);
                    player.DragonList[i].Offense = float.Parse(lines[8]);
                    player.DragonList[i].Defense = float.Parse(lines[9]);



                }
            }



            InventorySystem.Inventory playerInventory = Resources.Load<InventorySystem.Inventory>("ScriptableObjects/Inventories/PlayerInventory");

            if (SerializationManager.GetTextFromFile("PlayerInventory.txt", out lines)) {

                for (int i = 0; i < lines.Length; i++) {
                    playerInventory.Insert(lines[i]);
                }
            }

            ShopKeeperData weaponsShop = Resources.Load<ShopKeeperData>("ScriptableObjects/Shops/ArmorShop");
            if (SerializationManager.GetTextFromFile("WeaponsShop.txt", out lines)) {

                weaponsShop.Gold = int.Parse(lines[0]);
                for (int i = 1; i < lines.Length; i++) {
                    weaponsShop.items.Insert(lines[i]);
                }
            }

            ShopKeeperData goodsShop = Resources.Load<ShopKeeperData>("ScriptableObjects/Shops/Goods Shop");
            if (SerializationManager.GetTextFromFile("GoodsShop.txt", out lines)) {

                goodsShop.Gold = int.Parse(lines[0]);
                for (int i = 1; i < lines.Length; i++) {
                    goodsShop.items.Insert(lines[i]);
                }
            }
        }


        GlobalFlags.SetCurrentOverworldScene("Town - Interior");
        GlobalFlags.SetPlayerPosition(new Vector2(0.5f, 0f));
        GameManager.instance.PushState(StateManagement.GameStateType.Overworld, "Town - Interior");
        GlobalFlags.SetFirstTimeFlag(false);

    }

    public void OnOptionsPressed () {

        //NOTE: No longer a part of design specifications. 
        //Debug.Log("To do: implement options");
    }

    public void OnQuitPressed () {

        //Is there something to save????????????????? 
        if (GlobalFlags.PlayerData != null) {

            //Step 1: Save player inventory
            InventorySystem.Inventory inventoryDat = GlobalFlags.PlayerInventory;
            List<InventorySystem.Tuple> itemsSpace = inventoryDat.GetInventorySpace();
            List<string> lines = new List<string>();
            for (int i = 0; i < itemsSpace.Count; i++) {
                for (int j = 0; j < itemsSpace[i].Count; j++) {
                    lines.Add(itemsSpace[i].ItemName);
                }

            }

            SerializationManager.SaveTextToFile(lines.ToArray(), "PlayerInventory.txt");
            lines.Clear();

            

            //Step 2: Save player data
            DragonlordChroniclesDatabase.EntityData playerDat = GlobalFlags.PlayerData;

            lines.Add(playerDat.Level.ToString());
            lines.Add(playerDat.Experience.ToString());
            lines.Add(playerDat.CurrentHealth.ToString());
            lines.Add(playerDat.MaxHealth.ToString());
            lines.Add(playerDat.Offense.ToString());
            lines.Add(playerDat.Defense.ToString());
            lines.Add(playerDat.Speed.ToString());

            SerializationManager.SaveTextToFile(lines.ToArray(), "PlayerData.txt");
            lines.Clear();


            //Step 3: Save dragon data
            string[] dragonJson = new string[4];
            int idx = 0;
            foreach (var dragonDat in playerDat.DragonList) {

                lines.Add(dragonDat.DisplayName.ToString());
                lines.Add(dragonDat.Level.ToString());
                lines.Add(dragonDat.Experience.ToString());
                lines.Add(dragonDat.CurrentHealth.ToString());
                lines.Add(dragonDat.MaxHealth.ToString());
                lines.Add(dragonDat.CurrentMana.ToString());
                lines.Add(dragonDat.MaxMana.ToString());
                lines.Add(dragonDat.Magic.ToString());
                lines.Add(dragonDat.Speed.ToString());
                lines.Add(dragonDat.Offense.ToString());
                lines.Add(dragonDat.Defense.ToString());

                SerializationManager.SaveTextToFile(lines.ToArray(), "DragonData_" + idx.ToString() + ".txt");
                lines.Clear();

                idx++;
            }

            //Step 4: Save shopkeeper data
            ShopKeeperData weaponsShop = Resources.Load<ShopKeeperData>("ScriptableObjects/Shops/ArmorShop");

            lines.Add(weaponsShop.Gold.ToString());

            itemsSpace = weaponsShop.items.GetInventorySpace();
            for (int i = 0; i < itemsSpace.Count; i++) {
                for (int j = 0; j < itemsSpace[i].Count; j++) {
                    lines.Add(itemsSpace[i].ItemName);
                }
            }

            SerializationManager.SaveTextToFile(lines.ToArray(), "WeaponsShop.txt");
            lines.Clear();
            ShopKeeperData goodsShop = Resources.Load<ShopKeeperData>("ScriptableObjects/Shops/Goods Shop");

            lines.Add(goodsShop.Gold.ToString());

            itemsSpace = goodsShop.items.GetInventorySpace();
            for (int i = 0; i < itemsSpace.Count; i++) {
                for (int j = 0; j < itemsSpace[i].Count; j++) {
                    lines.Add(itemsSpace[i].ItemName);
                }
            }

            SerializationManager.SaveTextToFile(lines.ToArray(), "GoodsShop.txt");
            lines.Clear();
        }



        //NOTE: this only works on a build of the game.

        Application.Quit();
    }
    
}
