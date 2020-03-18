using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragonlordChroniclesDatabase
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Scriptable Objects/Spell")]
    public class SpellData : ScriptableObject
    {
        public enum SpellClass
        {
            Offense,
            Defense,
            HealHealth,
            RestoreMana
            //StatBuff
            //cause/heal status effects
        }

        [SerializeField]
        public int SpellID;
        [SerializeField]
        private string SpellName;
        [SerializeField]
        private SpellClass spellClass;
        [SerializeField]
        private float ManaCost;
        [SerializeField]
        private float SpellPower; //generic value for how much damage the spell inflicts, defends from, or heals, or how much mana it restores
        [SerializeField]
        private int NumberOfTargets;

        //status effects?

        public int GetSpellID()
        {
            return SpellID;
        }

        public string GetSpellName()
        {
            return SpellName;
        }

        public SpellClass GetSpellClass()
        {
            return spellClass;
        }

        public float GetManaCost()
        {
            return ManaCost;
        }

        public float GetSpellPower()
        {
            return SpellPower;
        }

        public int GetNumberOfTargets()
        {
            return NumberOfTargets;
        }
    }

}
