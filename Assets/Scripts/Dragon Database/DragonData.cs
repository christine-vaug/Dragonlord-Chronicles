using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragonlordChroniclesDatabase {

    public enum DragonType {
        Basic,
        Fire,
        Water,
        Grass
    }

    /*public enum DragonTier {
        A,
        B,
        C
    }*/

    

    /// <summary>
    /// This class is a container for the type of data that represents a dragon
    /// </summary>
    [CreateAssetMenu(fileName = "New Dragon", menuName = "Scriptable Objects/Dragon")]
    public class DragonData : EntityData {

        public DragonType element;
        //public DragonTier tier;
        //public List<SpellData> Spells;

        /*public float GetCurrentMana()
        {
            return CurrentMana;
        }

        public float GetMaxMana()
        {
            return MaxMana;
        }

        public void UseSpell(float mana)
        {
            if (mana >= CurrentMana)
            {
                CurrentMana = 0;
            }
            else
            {
                CurrentMana -= mana;
            }
        }

        public float GetMagic()
        {
            return Magic;
        }

        public DragonTier GetTier()
        {
            return Tier;
        }

        public void GainTier()
        {
            Tier++;
            Magic += 10;
        }*/
    }
}


