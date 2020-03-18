using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    /// <summary>
    /// InventorySystem class which holds item prefab slot data
    /// </summary> 
    public class Tuple
    {
        public string ItemName;
        public int Count;
        public bool Equipped;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Item's name</param>
        /// <param name="count">Item's count</param>
        /// <param name="equipped">Whether Item is equipped</param>
        public Tuple(string name, int count, bool equipped)
        {
            ItemName = name;
            Count = count;
            Equipped = equipped;
        }
    }
}
