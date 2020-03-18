using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// Class which contains database of all items in game
    /// </summary>
    [CreateAssetMenu(fileName = "New ItemDataBase", menuName = "Scriptable Objects/ItemDataBase")]
    public class ItemDataBase : ScriptableObject
    {
        /******************************************************
         *           To Add Items Into The Database
         ******************************************************
         *      1. Create field of type Item
         *      2. Call CreateInstance on type Item
         *      3. Call SetFields on that Item with desired parameters
         *      4. Add key value pair with the name as key and Item as value
         */
        
        //Healing Items
        Item apple;
        Item fullrestore;
        Item lesserpotion;
        Item potion;
        Item greaterpotion;
        Item mushroom;

        //Weapons
        Item axe;
        Item greataxe;
        Item lance;
        Item rustysword;
        Item sword;
        Item knightsword;
        Item bossbuster;

        //Head
        Item helmet;
        Item steelhelmet;
        Item platinumhelmet;

        //Chest
        Item chestplate;
        Item steelchestplate;
        Item platinumchestplate;

        //Hands
        Item gloves;
        Item steelgloves;
        Item platinumgloves;

        //Feet
        Item boots;
        Item steelboots;
        Item platinumboots;

        public Dictionary<string, Item> database;

        /// <summary>
        /// Method to populate database with items
        /// </summary>
        private void OnEnable()
        {
            //Creating Instances of items//

            //Healing Items
            apple = (Item)ScriptableObject.CreateInstance(typeof(Item));
            fullrestore = (Item)ScriptableObject.CreateInstance(typeof(Item));
            lesserpotion = (Item)ScriptableObject.CreateInstance(typeof(Item));
            potion = (Item)ScriptableObject.CreateInstance(typeof(Item));
            greaterpotion = (Item)ScriptableObject.CreateInstance(typeof(Item));
            mushroom = (Item)ScriptableObject.CreateInstance(typeof(Item));

            //Weapons
            axe = (Item)ScriptableObject.CreateInstance(typeof(Item));
            greataxe = (Item)ScriptableObject.CreateInstance(typeof(Item));
            lance = (Item)ScriptableObject.CreateInstance(typeof(Item));
            rustysword = (Item)ScriptableObject.CreateInstance(typeof(Item));
            sword = (Item)ScriptableObject.CreateInstance(typeof(Item));
            knightsword = (Item)ScriptableObject.CreateInstance(typeof(Item));
            bossbuster = (Item)ScriptableObject.CreateInstance(typeof(Item));

            //Head
            helmet = (Item)ScriptableObject.CreateInstance(typeof(Item));
            steelhelmet = (Item)ScriptableObject.CreateInstance(typeof(Item));
            platinumhelmet = (Item)ScriptableObject.CreateInstance(typeof(Item));

            //Chest
            chestplate = (Item)ScriptableObject.CreateInstance(typeof(Item));
            steelchestplate = (Item)ScriptableObject.CreateInstance(typeof(Item));
            platinumchestplate = (Item)ScriptableObject.CreateInstance(typeof(Item));

            //Hands
            gloves = (Item)ScriptableObject.CreateInstance(typeof(Item));
            steelgloves = (Item)ScriptableObject.CreateInstance(typeof(Item));
            platinumgloves = (Item)ScriptableObject.CreateInstance(typeof(Item));

            //Feet
            boots = (Item)ScriptableObject.CreateInstance(typeof(Item));
            steelboots = (Item)ScriptableObject.CreateInstance(typeof(Item));
            platinumboots = (Item)ScriptableObject.CreateInstance(typeof(Item));

            //Setting fields of items//

            //Healing Items
            apple.SetFields("Apple", "A red fruit that recovers 2 hp.", 1, 1.0f, 3, ItemType.Healing, false, false);
            fullrestore.SetFields("Full Restore", "Restores full hp.", 1, 5.0f, int.MaxValue,ItemType.Healing, false, false);
            lesserpotion.SetFields("Lesser Potion", "Less potent medicinal potion that recovers 5 hp.", 1, 5.0f, 5, ItemType.Healing, false, false);
            potion.SetFields("Potion", "Medicinal potion that recovers 10 hp.", 1, 5.0f, 10,ItemType.Healing, false, false);
            greaterpotion.SetFields("Greater Potion", "Very potent medicinal potion that recovers 20 hp.", 1, 5.0f, 20, ItemType.Healing, false, false);
            mushroom.SetFields("Mushroom", "Fungus that recovers 2 hp.", 1, 1.0f, 3,ItemType.Healing, false, false);

            //Weapons
            axe.SetFields("Axe", "A wood chopping axe when equipped adds 2 offense.", 1, 3.0f, 2, ItemType.Offense, false, false);
            greataxe.SetFields("Great Axe", "A battle axe when equipped adds 4 offense.", 1, 3.0f, 4, ItemType.Offense, false, false);
            lance.SetFields("Lance", "A knight's lance when equipped adds 15 offense.", 1, 8.0f, 15, ItemType.Offense, false, false);
            rustysword.SetFields("Rusty Sword", "A rusted sword when equipped adds 1 offense.", 1, 5.0f, 1, ItemType.Offense, false, false);
            sword.SetFields("Sword", "A sword when equipped adds 5 offense.", 1, 5.0f, 5, ItemType.Offense, false, false);
            knightsword.SetFields("Knight Sword", "A knight's sword when equipped adds 10 offense.", 1, 5.0f, 10, ItemType.Offense, false, false);
            bossbuster.SetFields("Boss Killer Sword", "You've hit the jackpot, a sword when equipped adds 100 offense", 1, 5.0f, 100, ItemType.Offense, false, false);

            //Head
            helmet.SetFields("Helmet", "Wool helmet when equipped adds 1 defense.", 1, 4.0f, 1, ItemType.HeadDefense, false, false);
            steelhelmet.SetFields("Steel Helmet", "Steel helmet when equipped adds 3 defense.", 1, 4.0f, 3, ItemType.HeadDefense, false, false);
            platinumhelmet.SetFields("Platinum Helmet", "Platinum helmet when equipped adds 5 defense.", 1, 4.0f, 5, ItemType.HeadDefense, false, false);

            //Chest
            chestplate.SetFields("Chest Plate", "Leather vest when equipped adds 2 defense.", 1, 5.0f, 2, ItemType.ChestDefense, false, false);
            steelchestplate.SetFields("Steel Chest Plate", "Steel chest plate when equipped adds 4 defense.", 1, 5.0f, 4, ItemType.ChestDefense, false, false);
            platinumchestplate.SetFields("Platinum Chest Plate", "Platinum chest plate when equipped adds 8 defense.", 1, 5.0f, 8, ItemType.ChestDefense, false, false);

            //Hands
            gloves.SetFields("Gloves", "Leather gloves when equipped adds 1 defense.", 1, 2.0f, 1, ItemType.HandDefense, false, false);
            steelgloves.SetFields("Steel Gloves", "Steel gloves when equipped adds 3 defense.", 1, 2.0f, 3, ItemType.HandDefense, false, false);
            platinumgloves.SetFields("Platinum Gloves", "Platinum gloves when equipped adds 5 defense.", 1, 2.0f, 5, ItemType.HandDefense, false, false);

            //Feet
            boots.SetFields("Boots", "Wool boots when equipped adds 1 defense.", 1, 2.0f, 1, ItemType.FootDefense, false, false);
            steelboots.SetFields("Steel Boots", "Steel boots when equipped adds 3 defense.", 1, 2.0f, 3, ItemType.FootDefense, false, false);
            platinumboots.SetFields("Platinum Boots", "Platinum boots when equipped adds 5 defense.", 1, 2.0f, 5, ItemType.FootDefense, false, false);

            //Adding to database
            database = new Dictionary<string, Item>()
            {
                //Healing Items
                {"Apple", apple},

                {"Full Restore", fullrestore},

                {"Lesser Potion", lesserpotion},

                {"Potion", potion},

                {"Greater Potion", greaterpotion},

                {"Mushroom", mushroom},

                //Weapons
                { "Axe", axe},

                { "Great Axe", greataxe},

                {"Lance", lance},

                {"Rusty Sword", rustysword},

                {"Sword", sword},

                {"Knight Sword", knightsword},

                {"Boss Killer Sword", bossbuster},

                //Head
                {"Helmet", helmet},

                {"Steel Helmet", steelhelmet},

                {"Platinum Helmet", platinumhelmet},

                //Chest
                {"Chest Plate", chestplate},

                {"Steel Chest Plate", steelchestplate},

                {"Platinum Chest Plate", platinumchestplate},

                //Hands
                {"Gloves", gloves},

                {"Steel Gloves", steelgloves},

                {"Platinum Gloves", platinumgloves},

                //Feet
                {"Boots", boots},

                {"Steel Boots", steelboots},

                {"Platinum Boots", platinumboots}
            };
        }
    }
}
