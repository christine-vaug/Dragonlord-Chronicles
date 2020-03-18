using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    //Enumeration for different types of items.
    public enum ItemType
    {
        Healing = 0,
        Defense = 1,
        Offense = 2,
        Key = 4,
        HeadDefense = 9,
        ChestDefense = 15,
        HandDefense = 33,
        FootDefense = 65
    };

    /// <summary>
    /// Class which holds data for inventory items
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject
    {
        public string Name;
        public string Description;
        public int Count;
        public float Value;
        public int Stat;
        public ItemType Type;
        public bool Equipped;
        public bool Unique;

        /// <summary>
        /// Method which sets all item fields
        /// </summary>
        /// <param name="name">Item's name</param>
        /// <param name="description">Item's description</param>
        /// <param name="count">Item's count</param>
        /// <param name="value">Item's cost</param>
        /// <param name="stat">Item's health, attack, defense depending on item's type</param>
        /// <param name="type">Item's healing, offense, defense type</param>
        /// <param name="equipped">Whether the item is equipped</param>
        /// <param name="unique">Whether the item is unique</param>
        public void SetFields(string name, string description, int count, float value,
            int stat, ItemType type, bool equipped, bool unique)
        {
            Name = name;
            Description = description;
            Count = count;
            Value = value;
            Stat = stat;
            Type = type;
            Equipped = equipped;
            Unique = unique;
        }
    }
}

