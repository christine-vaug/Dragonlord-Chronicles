using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonlordChroniclesDatabase;


namespace InventorySystem
{
    /// <summary>
    /// Class which manages the inventory's canvas in the Unity UI 
    /// </summary>
    public class InventoryCanvasController : MonoBehaviour
    {
        //Inventory canvas controller is static, so there is only one instance of it in the entire game
        public static InventoryCanvasController instance {get; private set;}
        public GameObject InventoryWindow;
        public GameObject ItemDialog;
        public GameObject CharacterStats;
        public GameObject ItemDescription;
        public GameObject DragonStatsImage;
        public GameObject DragonPartyImage;
        public InventoryUI InventoryStartUp;
        public Button ExitButton;
        public Button UseButton;
        public Button CancelButton;
        //public EntityData Player;
        public Toggle Slot;

        public bool OverWorld = false;
        public bool Combat = false;

        /// <summary>
        /// Unity Method called when the game starts
        /// </summary>
        private void Awake()
        {
            //Ensures that only the first instance made is the one that persists
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                gameObject.SetActive(false);
                ItemDialog.gameObject.SetActive(false);
                InventoryStartUp = (InventoryUI)InventoryWindow.GetComponent(typeof(InventoryUI));
                CancelButton.onClick.AddListener(CancelClick);
                UseButton.onClick.AddListener(UseClick);

                DragonImageControl TempImage = DragonPartyImage.GetComponent<DragonImageControl>();
                Toggle DragonToggle0 = TempImage.DragonImage0.GetComponent<Toggle>();
                DragonToggle0.onValueChanged.AddListener(delegate { NewActiveDragon(0); });
                Toggle DragonToggle1 = TempImage.DragonImage1.GetComponent<Toggle>();
                DragonToggle1.onValueChanged.AddListener(delegate { NewActiveDragon(1); });
                Toggle DragonToggle2 = TempImage.DragonImage2.GetComponent<Toggle>();
                DragonToggle2.onValueChanged.AddListener(delegate { NewActiveDragon(2); });
                Toggle DragonToggle3 = TempImage.DragonImage3.GetComponent<Toggle>();
                DragonToggle3.onValueChanged.AddListener(delegate { NewActiveDragon(3); });
            }

            else
            {
                Destroy(gameObject);
            }

        }

        /// <summary>
        /// Unity Method that is called on start up 
        /// </summary>
        void Start ()
        {
            ExitButton.onClick.AddListener(ExitButtonClick);
            UseButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);

        }

        /// <summary>
        /// Unity Method that is called once per frame
        /// </summary>
        void Update ()
        {
            //If anywhere gets clicked deactivate that dialogue
            if (Input.anyKeyDown)
            {
                ItemDialog.gameObject.SetActive(false);
            }
	    }

        /// <summary>
        /// Method which makes inventory canvas visible
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            CharacterStats.SetActive(true);
            InventoryStartUp.DisplayStats();
            InventoryStartUp.DisplayDragonParty();
            InventoryStartUp.DisplayNewActiveDragon(InventoryStartUp.Player.activeDragon);
        }

        /// <summary>
        /// Method which makes inventory canvas invisible
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
            ItemDialog.SetActive(false);
            UseButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);
            CharacterStats.SetActive(false);
            InventoryStartUp.ClearItemDescription();
        }

        /// <summary>
        /// Method which items are rewarded after battle
        /// </summary>
        public void InsertReward()
        {
            System.Random Reward = new System.Random();
            InventoryStartUp.ItemInventory.Insert(SelectItem(Reward.Next(100)));
            InventoryStartUp.CreateInventorySlot();
        }

        /// <summary>
        /// Method which selects random item from itemdatabase
        /// </summary>
        /// <param name="RewardNum">Random number</param>
        /// <returns>Returns item name</returns>
        //private string SelectItem(int RewardNum)
        //{
        //    switch (RewardNum)
        //    {
        //        case 0:
        //            return "Apple";
        //        case 1:
        //            return "Chest Plate";
        //        case 2:
        //            return "Axe";
        //        case 3:
        //            return "Boots";
        //        case 4:
        //            return "Helmet";
        //        case 5:
        //            return "Lance";
        //        case 6:
        //            return "Elixir";
        //        case 7:
        //            return "Mushroom";
        //        case 8:
        //            return "Potion";
        //        case 9:
        //            return "Sword";
        //        case 10:
        //            return "Gloves";
        //        default:
        //            return "out of bounds";
        //    }
        //}

        private string SelectItem(int RewardNum)
        {
            Debug.Log(RewardNum);
            System.Random ChooseItem = new System.Random();
            int Selecting;

            if (RewardNum <= 69)
            {
                Selecting = ChooseItem.Next(5);

                switch (Selecting)
                {
                    case 0:
                        return "Apple";
                    case 1:
                        return "Mushroom";
                    case 2:
                        return "Potion";
                    case 3:
                        return "Lesser Potion";
                    case 4:
                        return "Greater Potion";
                    default:
                        return "Apple";
                }
            }

            if(RewardNum > 69 && RewardNum <= 79)
            {
                return "Full Restore";
            }

            else
            {
                if(RewardNum == 99)
                {
                    return "Boss Killer Sword";
                }

                if(RewardNum >= 80 && RewardNum < 94)
                {
                    Selecting = ChooseItem.Next(6);
                    switch (Selecting)
                    {
                        case 0:
                            return "Rusty Sword";
                        case 1:
                            return "Axe";
                        case 2:
                            return "Chest Plate";
                        case 3:
                            return "Gloves";
                        case 4:
                            return "Boots";
                        case 5:
                            return "Helmet";
                        default:
                            return "Rusty Sword";
                    }
                }

                else
                {
                    Selecting = ChooseItem.Next(6);
                    switch (Selecting)
                    {
                        case 0:
                            return "Sword";
                        case 1:
                            return "Great Axe";
                        case 2:
                            return "Steel Chest Plate";
                        case 3:
                            return "Steel Gloves";
                        case 4:
                            return "Steel Boots";
                        case 5:
                            return "Steel Helmet";
                        default:
                            return "Sword";
                    }
                }
            }
        }

        /// <summary>
        /// Method that handles exit button in Unity UI
        /// </summary>
        public void ExitButtonClick()
        {
            Deactivate();
            UseButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(false);
            Combat = false;
        }

        /// <summary>
        /// Method that handles Use Click in overworld
        /// </summary>
        public void UseClick()
        {
            if (OverWorld)
            {
                //UseItem gets called with item that was clicked on 
                InventoryStartUp.UseItem(Slot.gameObject.name, Slot);
                InventoryStartUp.CreateInventorySlot();
                UseButton.gameObject.SetActive(false);
                CancelButton.gameObject.SetActive(false);
                InventoryStartUp.ClearItemDescription();
            }
        }

        /// <summary>
        /// Method that handle Cancel Click in overworld
        /// </summary>
        public void CancelClick()
        {
            if (OverWorld)
            {
                UseButton.gameObject.SetActive(false);
                CancelButton.gameObject.SetActive(false);
                InventoryStartUp.ClearItemDescription();
            }
        }

        /// <summary>
        /// Method which sends slot that was clicked on in inventory UI into the canvas controller
        /// </summary>
        /// <param name="slot">Item clicked on</param>
        public void SetToggleSlot(Toggle slot)
        {
            Slot = slot;
        }

        public void NewActiveDragon(int DragonToggle)
        {
            InventoryStartUp.DisplayNewActiveDragon(DragonToggle);
        }
    }
}
