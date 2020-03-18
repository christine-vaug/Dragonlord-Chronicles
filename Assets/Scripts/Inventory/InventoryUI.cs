using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonlordChroniclesDatabase;

namespace InventorySystem
{
    /// <summary>
    /// Class which handles the behavior of the Inventory UI
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public Inventory ItemInventory;
        public ItemDataBase itemDatabase;
        public EntityData Player;
        //public EntityData Dragon;

        public GameObject ItemSlotPreFab;
        public GameObject Content;
        public ToggleGroup ItemSlotToggle;

        private int x = 0;
        private int y = 0;
        private GameObject ItemSlot;

        /// <summary>
        /// Method which is called implicitly by Unity on start up
        /// </summary>
        public void Awake()
        {
            //creation of the Inventory and the ItemDataBase
            ItemInventory = Resources.Load<Inventory>("ScriptableObjects/Inventories/PlayerInventory");
            itemDatabase = Resources.Load<ItemDataBase>("ScriptableObjects/ItemDataBase");
            //Dragon = Resources.Load<DragonData>("ScriptableObjects/Dragons");
            GlobalFlags.PlayerInventory = ItemInventory;


            Player.ItemUsed = string.Empty;

            //Initially draws the inventory slots
            CreateInventorySlot();
        }



        // Use this for initialization
        void Start()
        {
            //ItemInventory.Insert("Potion");
        }

        // Update is called once per frame
        void Update()
        {
          
        }

        /// <summary>
        /// Method that draws inventory slots 
        /// </summary>
        public void CreateInventorySlot()
        {
            Clear();

            //Creates list for Toggles and Tuples
            List<Toggle> ToggleSlots = new List<Toggle>();
            List<Tuple> ItemList = ItemInventory.GetInventorySpace(false);

            //Iterates through the tuple list and draws the slots
            for (int i = 0; i < ItemList.Count; i++)
            {
                ItemSlot = (GameObject)Instantiate(ItemSlotPreFab);
                TextControl TextName = ItemSlot.GetComponent<TextControl>();

                if (ItemList[i].Equipped)
                {
                    TextName.Arrow.text = "(E)";
                }

                else
                {
                    TextName.Arrow.text = "";

                }

                if(ItemList[i].Count > 1)
                {
                    TextName.Count.text = "x" + ItemList[i].Count.ToString();
                }

                else
                {
                    TextName.Count.text = "";

                }

                //Name drawn on the Slot
                TextName.ItemName.text = ItemList[i].ItemName;
                //Name of the Slot
                ItemSlot.name = ItemList[i].ItemName;

                //Adds item into scrollable Unity UI and builds toggle list
                ItemSlot.GetComponent<Toggle>().group = ItemSlotToggle;
                ToggleSlots.Add(ItemSlot.GetComponent<Toggle>());
                ItemSlot.transform.SetParent(Content.transform, false);
                ItemSlot.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                y -= (int)ItemSlot.GetComponent<RectTransform>().rect.height;
            }
                
            //Go through list of toggles and add click listeners to each one 
            foreach(var SelectingItem in ToggleSlots)
            {
                SelectingItem.onValueChanged.AddListener(delegate { GetInventoryItem(SelectingItem); });
            }
        }

        /// <summary>
        /// Method to clear inventory slots 
        /// </summary>
        public void Clear()
        {
            int size = Content.transform.childCount;

            for(int i = 0; i< size; i++)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Method which is called when a slot is clicked on  
        /// </summary>
        /// <param name="Slot">The toggle on the slot</param>
        public void GetInventoryItem(Toggle Slot)
        {
            TextControl TempText = InventoryCanvasController.instance.UseButton.GetComponent<TextControl>();

            Item TempItem = itemDatabase.database[Slot.gameObject.name];

            Item PreviousItem = ItemInventory.CheckEquipped(TempItem.Type);

            if (((TempItem.Type == ItemType.Offense || ((int)TempItem.Type & 1) == (int)ItemType.Defense)))
            {
                if (PreviousItem != null && PreviousItem.Name == TempItem.Name)
                {
                    TempText.Arrow.text = "Unequip";
                }

                else
                {
                    TempText.Arrow.text = "Equip";
                }
            }

            else
            {
                TempText.Arrow.text = "Use";
            }


            InventoryCanvasController.instance.UseButton.gameObject.SetActive(true);
            InventoryCanvasController.instance.CancelButton.gameObject.SetActive(true);
            DisplayItemDescription(Slot);

            //if in the overworld pass the name of the item clicked on into the inventory canvas controller
            if (InventoryCanvasController.instance.OverWorld)
            {
                InventoryCanvasController.instance.SetToggleSlot(Slot);
            }

            //if in the combat pass the name of the item to the player
            else if (InventoryCanvasController.instance.Combat)
            {

                Player.ItemUsed = Slot.gameObject.name;
            }
        }

        /// <summary>
        /// Method that calls Use in inventory and checks whether item is a healing or equippable item
        /// </summary>
        /// <param name="ItemName">Item's name</param>
        /// <param name="UseSlot">Optional parameter defaulted to null which is the item that was clicked on</param>
        /// <returns>Returns whether or not use was successful</returns>
        public bool UseItem(string ItemName, Toggle UseSlot = null)
        {
            //Get Item from the database based on the name
            Item TempItem = itemDatabase.database[ItemName];
            
            //Getting Unity's UI text image and setting it to active 
            TextControl TempText = InventoryCanvasController.instance.ItemDialog.GetComponent<TextControl>();
            InventoryCanvasController.instance.ItemDialog.gameObject.SetActive(true);

            //If it's a healing item, use item, heal player, draw text for itemdialog and stats 
            if (Player.GetCurrentHealth() < Player.GetMaxHealth() && TempItem.Type == ItemType.Healing)
            {
                ItemInventory.Use(TempItem.Name);
                Player.Heal((float)TempItem.Stat);

                if(TempItem.Name == "Full Restore")
                {
                    TempText.ItemDialog.text = "Used " + TempItem.Name + " and healed max health " + "\n" +
                    "Your health is now " + Player.GetCurrentHealth();
                }

                else
                {
                    TempText.ItemDialog.text = "Used " + TempItem.Name + " and healed " + TempItem.Stat + " health " + "\n" + 
                    "Your health is now " + Player.GetCurrentHealth();
                }

                DisplayStats();
                return true;
            }

            //if it's an equippable item, equip it and display stats
            else if((TempItem.Type == ItemType.Offense || ((int)TempItem.Type & 1) == (int)ItemType.Defense) && InventoryCanvasController.instance.OverWorld)
            {
                EquipItem(TempItem, UseSlot, TempText);
                DisplayStats();
                return true;
            }

            TempText.ItemDialog.text = "You can't use this right now!";
            return false;
        }

        /// <summary>
        /// Method to equip items
        /// </summary>
        /// <param name="ItemName">Item to equip</param>
        /// <param name="UseSlot">Slot that was clicked on</param>
        /// <param name="TempText">Itemdialog text</param>
        public void EquipItem(Item ItemName, Toggle UseSlot, TextControl TempText)
        {
            Item PreviousItem = ItemInventory.CheckEquipped(ItemName.Type);

            //If no item was equipped previously, then equip item
            if (PreviousItem == null)
            {
                ItemInventory.ChangeEquipped(ItemName, true);
                
                //Adds stats in entitydata
                Player.SetEquipStat(Player.GetEquipStat((int)ItemName.Type) + (float)ItemName.Stat, (int)ItemName.Type);
                TempText.ItemDialog.text = ItemName.Name + " Equipped";
            }

            else if(UseSlot != null)
            {
                TextControl TextName = UseSlot.GetComponent<TextControl>();

                //If the previous item that is equipped is the same as the item being clicked on, then unequip item
                if (PreviousItem.Name == ItemName.Name && TextName.Arrow.text != "")
                {
                    ItemInventory.ChangeEquipped(PreviousItem, false);

                    //Reverts stats in entitydata
                    Player.SetEquipStat(Player.GetEquipStat((int)PreviousItem.Type) - (float)PreviousItem.Stat, (int)PreviousItem.Type);
                    TempText.ItemDialog.text = ItemName.Name + " Unequipped";
                }

                //If previous item equipped is different from the current item being clicked on
                //then unequip previous item and equip current
                else if (PreviousItem.Name != ItemName.Name)
                {
                    ItemInventory.ChangeEquipped(PreviousItem, false);
                    Player.SetEquipStat(Player.GetEquipStat((int)PreviousItem.Type) - (float)PreviousItem.Stat, (int)PreviousItem.Type);
                    ItemInventory.ChangeEquipped(ItemName, true);
                    Player.SetEquipStat(Player.GetEquipStat((int)ItemName.Type) + (float)ItemName.Stat, (int)ItemName.Type);
                    TempText.ItemDialog.text = PreviousItem.Name + " Unequipped" + " and " + ItemName.Name + " Equipped";
                }
            }
        }

        /// <summary>
        /// Method which displays character's current stats in Inventory Unity UI
        /// </summary>
        public void DisplayStats()
        {
            TextControl TextName = InventoryCanvasController.instance.CharacterStats.GetComponent<TextControl>();
            List<float> Stats = Player.ReturnStats();

            if (Stats != null && Stats.Count > 10)
            {
                TextName.ItemDialog.text = "HP: " + Stats[0] + "/" + Stats[1] + "\n" +
                    "Offense: " + Stats[4] + "\n" +
                    "Defense: " + Stats[5] + "\n" +
                    "Speed: " + Stats[7] + "\n";

                TextName.Count.text = "LVL: " + Stats[8] + "\n" + "EXP: " + Stats[9] + "/" + "1000" + "\n" + "Gold: " + Stats[10]; 
            }
                
        }

        /// <summary>
        /// Method that writes the currently selected item's description
        /// </summary>
        /// <param name="Slot">Currently selected item</param>
        public void DisplayItemDescription(Toggle Slot)
        {
            TextControl TempText = InventoryCanvasController.instance.ItemDescription.GetComponent<TextControl>();
            Item TempItem = itemDatabase.database[Slot.gameObject.name];
            TempText.Arrow.text = TempItem.Description;
        }

        /// <summary>
        /// Method that clears item description after item is unselected or used
        /// </summary>
        public void ClearItemDescription()
        {
            TextControl TempText = InventoryCanvasController.instance.ItemDescription.GetComponent<TextControl>();
            TempText.Arrow.text = "";
        }

        public void DisplayNewActiveDragon(int index)
        {
            if(Player.DragonList[index] != null)
            {
                TextControl TempText = InventoryCanvasController.instance.DragonStatsImage.GetComponent<TextControl>();

                if (InventoryCanvasController.instance.OverWorld)
                {
                    TempText.ItemDialog.text = "Current Active Dragon";
                    Player.ChangeActiveDragon(index);
                }

                else
                {
                    TempText.ItemDialog.text = "Dragon Status";
                }

                List<float> Stats = Player.DragonList[index].ReturnStats();

                if (Stats != null && Stats.Count > 10)
                {
                    TempText.Count.text = "HP: " + Stats[0] + "/" + Stats[1] + "\n" +
                        "MP: " + Stats[2] + "/" + Stats[3] + "\n" +
                        "Offense: " + Stats[4] + "\n" +
                        "Defense: " + Stats[5] + "\n" +
                        "Magic: " + Stats[6] + "\n" +
                        "Speed: " + Stats[7] + "\n";

                    TempText.Arrow.text = "LVL: " + Stats[8] + "\n" + "EXP: " + Stats[9] + "/" + "1000" + "\n";
                }

                TempText.ItemName.text = Player.GetDragonNameFromList(index);
                DragonImageControl TempImage = InventoryCanvasController.instance.DragonStatsImage.GetComponent<DragonImageControl>();
                TempImage.DragonImage0.sprite = Player.DragonList[index].battleSprite;
            }
        }

        public void DisplayDragonParty()
        {

            DragonImageControl TempImage = InventoryCanvasController.instance.DragonPartyImage.GetComponent<DragonImageControl>();

            if(Player.numDragons > 0 && Player.DragonList[0] != null)
            {
                TempImage.DragonImage0.gameObject.SetActive(true);
                TempImage.DragonImage0.sprite = Player.DragonList[0].battleSprite;
            }

            else
            {
                TempImage.DragonImage0.gameObject.SetActive(false);
            }

            if (Player.numDragons > 1 && Player.DragonList[1] != null)
            {
                TempImage.DragonImage1.gameObject.SetActive(true);
                TempImage.DragonImage1.sprite = Player.DragonList[1].battleSprite;
            }

            else
            {
                TempImage.DragonImage1.gameObject.SetActive(false);
            }

            if (Player.numDragons > 2 && Player.DragonList[2] != null)
            {
                TempImage.DragonImage2.gameObject.SetActive(true);
                TempImage.DragonImage2.sprite = Player.DragonList[2].battleSprite;
            }

            else
            {
                TempImage.DragonImage2.gameObject.SetActive(false);
            }

            if (Player.numDragons > 3 && Player.DragonList[3] != null)
            {
                TempImage.DragonImage3.gameObject.SetActive(true);
                TempImage.DragonImage3.sprite = Player.DragonList[3].battleSprite;
            }

            else
            {
                TempImage.DragonImage3.gameObject.SetActive(false);
            }
        }
        
    }
}