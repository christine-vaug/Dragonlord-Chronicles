using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DragonlordChroniclesDatabase {

    [CreateAssetMenu(fileName = "New Entity Container", menuName = "Scriptable Objects/Entity")]

    public class EntityData : ScriptableObject {

        public string DisplayName;
        public Sprite battleSprite;

        public float CurrentHealth;
        public float MaxHealth;
        public float CurrentMana;
        public float MaxMana;
        public bool Dead = false;

        public float Offense;
        public float Defense;
        public float Magic;
        public float Speed;

        public float TempOffense = 1.0f;
        public float TempDefense = 1.0f;
        public float TempMagic = 1.0f;
        public float TempSpeed = 1.0f;

        public float Level;
        public float Experience;
        public float Tier;

        public float Gold;
        //public InventorySystem.ItemList items;

        public SpellData[] Spells = new SpellData[4]; //each magic-using entity can have up to 4 spells at a time
        public int numSpells; //the number of spells the entity currently knows
        public int spellIndex;

        public DragonData[] DragonList = new DragonData[4]; //player can have up to 4 dragons with them at a time
        public int activeDragon; //the index of the active dragon
        public int numDragons; //the total number of dragons the player has with them

        public string ItemUsed = string.Empty;

        public void SetStats(string name, float hp, float mana, float off, float def, float mgc, float spd, float lvl, float tier, float g)
        {
        	DisplayName = name;
        	MaxHealth = CurrentHealth = hp;
        	MaxMana = CurrentMana = mana;

        	Offense = off;
        	Defense = def;
        	Magic = mgc;
        	Speed = spd;

        	Level = lvl;
        	Tier = tier;

        	Gold = g;
        }

        public string GetName()
		{
			return DisplayName;
		}

		public void SetName(string name)
		{
			DisplayName = name;
		}

        public Sprite GetSprite()
        {
            return battleSprite;
        }

        public float GetCurrentHealth()
		{
			return CurrentHealth;
		}

		public void TakeDamage(float dam)
		{
			if(dam >= CurrentHealth)
			{
				CurrentHealth = 0;
				Dead = true;
			} else
			{
				CurrentHealth -= dam;
			}
		}

		public bool IsDead()
		{
			return CurrentHealth <= 0;
		}

		public void Heal(float hp)
		{
			if(CurrentHealth + hp > MaxHealth)
			{
				CurrentHealth = MaxHealth;
			}
			else
			{
				CurrentHealth += hp;
			}

			Dead = false;
		}

        public void SetCurrentHealth(float hp)
        {
            if (hp > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
            else if (hp < 0.0f)
            {
                CurrentHealth = 0.0f;
            }
            else
            {
                CurrentHealth = hp;
            }
        }

		public float GetMaxHealth()
		{
			return MaxHealth;
		}

        public float GetCurrentMana()
        {
            return CurrentMana;
        }

        public float GetMaxMana()
        {
            return MaxMana;
        }

        public void RestoreMana(float mana)
        {
            if (CurrentMana + mana > MaxMana)
            {
                CurrentMana = MaxMana;
            }
            else
            {
                CurrentMana += mana;
            }
        }

        public void SetCurrentMana(float mana)
        {
            if (mana > MaxMana)
            {
                CurrentMana = MaxMana;
            }
            else if (mana < 0.0f)
            {
                CurrentMana = 0.0f;
            }
            else
            {
                CurrentMana = mana;
            }
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

        public float GetOffense()
		{
            return Offense * TempOffense;
		}
        
        /// <summary>
        /// Method which gets offense or defense stats without modifier
        /// </summary>
        /// <param name="Control">Indicates offense or defense type</param>
        /// <returns>Returns stat</returns>
        public float GetEquipStat(int Control)
        {
            if(Control == (int)InventorySystem.ItemType.Offense)
            {
                return Offense;
            }

            else 
            {
                return Defense;
            }
        }

        /// <summary>
        /// Method which sets offense or defense stats without modifier
        /// </summary>
        /// <param name="Stat">The new offense or defense stat</param>
        /// <param name="Control">Indicates offense or defense type</param>
        public void SetEquipStat(float Stat, int Control)
        {
            if (Control == (int)InventorySystem.ItemType.Offense)
            {
                Offense = Stat;
            }

            else
            {
                Defense = Stat;
            }
        }

        /// <summary>
        /// Method which returns all stats without modifiers 
        /// </summary>
        /// <returns>Returns list of all stats</returns>
        public List<float> ReturnStats()
        {
            List<float> Output = new List<float>()
            {
                CurrentHealth,
                MaxHealth,
                CurrentMana,
                MaxMana,
                Offense,
                Defense,
                Magic,
                Speed,
                Level,
                Experience,
                Gold
            };

            return Output;
        }

		public float GetDefense()
		{
			return Defense * TempDefense;
		}

		public float GetMagic()
		{
			return Magic * TempMagic;
		}

		public float GetSpeed()
		{
			return Speed * TempSpeed;
		}

        public void ModifyOffense(float offset)
        {
            TempOffense = offset;
        }

        public void ResetOffense()
        {
            TempOffense = 1.0f;
        }

        public void ModifyDefense(float offset)
        {
            TempDefense = offset;
        }

        public void ResetDefense()
        {
            TempDefense = 1.0f;
        }

        public void ModifyMagic(float offset)
        {
            TempMagic = offset;
        }

        public void ResetMagic()
        {
            if (name == "Player")
                TempMagic = 0.0f;
            else
                TempMagic = 1.0f;
        }

        public void ModifySpeed(float offset)
        {
            TempSpeed = offset;
        }

        public void ResetSpeed()
        {
            TempSpeed = 1.0f;
        }

        public void ResetStats()
        {
            ResetOffense();
            ResetDefense();
            ResetMagic();
            ResetSpeed();
        }

		public float GetLevel()
		{
			return Level;
		}

		public float GetExperience()
		{
			return Experience;
		}

		public void GainExperience(float exp)
		{
			Experience += exp;

			if (Experience >= 1000)
			{
				LevelUp();
				Experience -= 1000;
			}
		}

		private void LevelUp()
		{
			Level++;

            float healthGained = Random.Range(5, 10);
			MaxHealth += healthGained;
			CurrentHealth += healthGained;
            if (name != "Player")
                GainTier();

			Offense += Random.Range(1, 3);
            Defense += Random.Range(1, 3);
            Speed += Random.Range(1, 3);
		}

		public float GetTier()
		{
			return Tier;
		}

		public void GainTier()
		{
			Tier++;
			Magic += Random.Range(1, 3);
		}

		public float GetGold()
		{
			return Gold;
		}

		public void AddGold(float g)
		{
			Gold += g;
		}

        public void AddSpell(string spell, int index)
        {
            if(numSpells < 4)
            {
                //numSpells will be at the correct index number until 4 spells are learned
                //Assigns the passed in spell by referencing the scriptable object in Resources/Spells/spell
                Spells[numSpells] = Resources.Load<SpellData>("ScriptableObjects/Spells/" + spell);
                numSpells++;
            }
            else if(numSpells >= 4)
            {
                Spells[index] = Resources.Load<SpellData>("ScriptableObjects/Spells/" + spell);
            }
        }

        public SpellData GetSpellByIndex(int index)
        {
            return Spells[index];
        }

        public int GetNumSpells()
        {
            return numSpells;
        }

        public void AddDragon(string dragonName, int index)
        {
            string path = "ScriptableObjects/Dragons/" + dragonName; 

            //numDragons will be at the correct index number until there are 4 dragons
            if (numDragons < 4)
            {
                //put new dragon in party
                DragonList[numDragons].SetDragonStatsFromResources(path);
                numDragons++;
                //if this is first dragon, it automatically becomes your active dragon
                if (numDragons == 1)
                   ChangeActiveDragon(0);
            }
            else
            {
                //put new dragon in party
                DragonList[index].SetDragonStatsFromResources(path);
            }
        }

        public EntityData GetActiveDragon()
        {
            return DragonList[activeDragon];
        }

        public int GetNumDragons()
        {
            return numDragons;
        }

        public string GetDragonNameFromList(int i)
        {
            if (numDragons <= i)
            {
                return "(Empty)";
            }
            else
            {
                return DragonList[i].DisplayName;
            }
        }

        public void ChangeActiveDragon(int i)
        {
            if(!DragonList[i].IsDead())
                activeDragon = i;
        }

        //returns true if all the player's dragons are dead, and false if at least one has > 0 HP
        public bool AllDragonsDead()
        {
            bool allDead = true;

            for(int i = 0; i < numDragons; i++)
            {
                if (!DragonList[i].IsDead())
                    allDead = false;
            }

            return allDead;
        }

        public void ResetToLevelOne()
        {
            CurrentHealth = MaxHealth = 10;
            if (name == "Player")
                CurrentMana = MaxMana = 0;
            else
                CurrentMana = MaxMana = 100;
            Dead = false;

            Offense = Random.Range(1, 3);
            Defense = Random.Range(1, 3);
            if (name == "Player")
                Magic = 0;
            else
                Magic = 10;
            Speed = Random.Range(5, 15);

            TempOffense = 1.0f;
            TempDefense = 1.0f;
            TempMagic = 1.0f;
            TempSpeed = 1.0f;

            Level = 1.0f;
            Experience = 0.0f;
            Tier = 1.0f;

            Gold = 0.0f;
        }

        public void ResetDragonList()
        {
            for (int i = 0; i < 4; i++)
            {
                DragonList[i] = Resources.Load<DragonData>("ScriptableObjects/Party/Slot" + i);
                DragonList[i].SetDragonStatsFromResources("ScriptableObjects/Dragons/Blank Dragon");
            }
            numDragons = 0;

            AddDragon("Basic Drakling", -1);
        }

        public void SetDragonStatsFromResources(string path)
        {
            DisplayName = Resources.Load<DragonData>(path).GetName();
            battleSprite = Resources.Load<DragonData>(path).GetSprite();
            MaxHealth = CurrentHealth = Resources.Load<DragonData>(path).GetMaxHealth();
            CurrentHealth = Resources.Load<DragonData>(path).GetCurrentHealth();
            MaxMana = CurrentMana = Resources.Load<DragonData>(path).GetMaxMana();
            CurrentMana = Resources.Load<DragonData>(path).GetCurrentMana();
            Dead = false;

            Offense = Resources.Load<DragonData>(path).GetOffense();
            Defense = Resources.Load<DragonData>(path).GetDefense();
            Magic = Resources.Load<DragonData>(path).GetMagic();
            Speed = Resources.Load<DragonData>(path).GetSpeed();

            ResetStats();

            Level = Resources.Load<DragonData>(path).GetLevel();
            Tier = Resources.Load<DragonData>(path).GetTier();

            Gold = Resources.Load<DragonData>(path).GetGold();

            numSpells = Resources.Load<DragonData>(path).GetNumSpells();

            for(int i =0; i < numSpells; i++)
            {
                Spells[i] = Resources.Load<DragonData>(path).GetSpellByIndex(i);
            }
        }
    }
}

