using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

namespace InventorySystem
{
    /// <summary>
    /// Class which manages the player's inventory
    /// </summary>
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Inventory")]
    public class Inventory : ScriptableObject
    {
        private Dictionary<string, Item> itemMap;
        private int size;

        /// <summary>
        /// Constructor
        /// </summary>
        public Inventory()
        {
            size = 0;
            itemMap = new Dictionary<string, Item>();
        }

        /// <summary>
        /// Method for inserting item into inventory
        /// </summary>
        /// <param name="inputItemData">the item to insert</param>
        /// <returns>bool corresponding to whether or not the insert was successful</returns>
        public bool Insert(string inputItemData, int Count = 1)
        {
            ItemDataBase itemDatabase = (ItemDataBase)ScriptableObject.CreateInstance(typeof(ItemDataBase));
            bool FirstInsert = false;

            //if the item is not null
            if (inputItemData != null)
            {
                //if the inventory contains the item already and the item is not unique, then increment
                //the item count, otherwise actually insert into the map, increment size
                if (!itemMap.ContainsKey(inputItemData))
                {
                    itemMap.Add(inputItemData, itemDatabase.database[inputItemData]);
                    size++;
                    FirstInsert = true;
                }

                if(!itemDatabase.database[inputItemData].Unique)
                {
                    if (FirstInsert == true)
                    {
                        Count--;
                    }

                    itemMap[inputItemData].Count += Count;

                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Method for removing items from the inventory
        /// </summary>
        /// <param name="itemName">the key of the item to remove</param>
        /// <returns>bool corresponding to whether or not the remove was successful</returns>
        public bool Remove(string itemName)
        {
            //if the item is in the inventory and the count is greater than zero and it's not a Key type item,
            //decrement the item count
            if(itemMap.ContainsKey(itemName) && itemMap[itemName].Count > 0 && itemMap[itemName].Type != ItemType.Key)
            {
                itemMap[itemName].Count--;

                //if the count went to zero, then the item should be removed, then decrement size
                if(itemMap[itemName].Count <= 0)
                {
                    itemMap.Remove(itemName);
                    size--;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to get an item from the inventory
        /// </summary>
        /// <param name="itemName">the key of the item to get</param>
        /// <returns>the item object</returns>
        public Item Get(string itemName)
        {
            //if the item is in the inventory and its count is greater than zero, return the item
            if(itemMap.ContainsKey(itemName) && itemMap[itemName].Count > 0)
            {

                return itemMap[itemName];
            }

            return null;
        }

        /// <summary>
        /// Method to use an item which gets and removes the item
        /// </summary>
        /// <param name="itemName">the key of the item to use</param>
        /// <returns>the item object to use</returns>
        public Item Use(string itemName)
        {
            //get the item, if its not null then remove it
            Item tempItem = Get(itemName);

            if(tempItem != null)
            {
                Remove(itemName);
            }

            return tempItem;
        }

        /// <summary>
        /// Method for getting the contents of the inventory as a list of Tuples containing
        /// the name of the item and its count
        /// </summary>
        /// <param name="Restricted">Bool that controls whether or not the entire Inventory is returned</param>
        /// <returns>returns a list of Tuples with the </returns>
        public List<Tuple> GetInventorySpace(bool Restricted = true)
        {
            List<Tuple> tupleList = new List<Tuple>();

            //for each item in the inventory, make a Tuple, set its fields to the name and count of the
            //current item and add the Tuple to the list
            foreach(var item in itemMap)
            {
                int ItemCount = item.Value.Count;

                //Only if the items are offense or defense multiple tuples are made
                //because items should not stack in Unity UI
                if (item.Value.Type == ItemType.Offense || ((int)item.Value.Type & 1) == (int)ItemType.Defense)
                {
                    bool TempEquip = false;

                    for (int i = 0; i < ItemCount; i++)
                    {
                        if(i == 0 && item.Value.Equipped)
                        {
                            TempEquip = true;
                        }

                        //Only add the tuple to the list if the item is not equipped or if caller wants full list
                        if ((Restricted && !TempEquip) || !Restricted)
                        {
                            var newTuple = new Tuple(item.Value.Name, 1, TempEquip);
                            tupleList.Add(newTuple);
                        }

                        TempEquip = false;
                    }
                }

                else
                {
                    var newTuple = new Tuple(item.Value.Name, item.Value.Count, item.Value.Equipped);
                    tupleList.Add(newTuple);
                }
                
            }

            return tupleList;
        }

        /// <summary>
        /// Method for getting the size of the the inventory
        /// </summary>
        /// <returns>the size of the inventory</returns>
        public int GetInventorySize()
        {
            return size;
        }

        /// <summary>
        /// Method which checks whether or not an item is equipped
        /// </summary>
        /// <param name="type">Item's Type</param>
        /// <returns>Returns the item of that type, unless it's not equipped then null</returns>
        public Item CheckEquipped(ItemType type)
        {
            //Iterating through inventory checking each item's type against the type that was passed in 
            //if there's a match then the item is returned
            foreach (var item in itemMap)
            {
                if(item.Value.Type == type && item.Value.Equipped)
                {
                    return item.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Method to equip or unequip an item
        /// </summary>
        /// <param name="ItemName">The item</param>
        /// <param name="Equip">Whether or not to equip the item</param>
        public void ChangeEquipped(Item ItemName, bool Equip)
        {
            itemMap[ItemName.Name].Equipped = Equip;
        }

        public void ResetInventory(bool player = true)
        {
            itemMap.Clear();
            if (player)
            {
                Insert("Lesser Potion");
            }

        }

        /// <summary>
        /// Method for printing out the contents of the Tuple list 
        /// </summary>
        public void DiagnosticTuplePrint()
        {
            //get the contents of the inventory in the form of a list
            List<Tuple> tupleList = GetInventorySpace();

            //iterate over the list and print out the item name and the count
            foreach(var item in tupleList)
            {
                Debug.Log("Name:" + item.ItemName + " Count: " + item.Count + " Equip: " + item.Equipped);
            }
        }

        /// <summary>
        /// Method for printing out the contents of the dictionary/map list 
        /// </summary>
        public void DiagnosticMapPrint()
        {
            //iterate over the map and print out the item data
            foreach (var item in itemMap)
            {
                Item tempItem = item.Value;
                Debug.Log(tempItem.Name + " " + tempItem.Description + " " + tempItem.Count + " " + 
                    tempItem.Value + " " + tempItem.Stat + " " + tempItem.Equipped.ToString() + " " + tempItem.Unique.ToString());
            }
        }
    }
}
