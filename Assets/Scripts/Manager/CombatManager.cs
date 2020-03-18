using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DragonlordChroniclesDatabase;
using InventorySystem;

// Manager for the combat system in the game

/***************************************************************
 *                  TO USE THE COMBAT MANAGER
 ***************************************************************
 * Step 1: Call either QueuePlayer OR QueueItem (NOT BOTH IN THE SAME ROUND)
 * Step 2: Call QueueDragon
 * Step 3: Call TakeRound()
 *
 * QueuePlayer takes a code representing the action the Player will perform
 * The codes for the Player are as follows:
 * 0 - No action (default)
 * 1 - Attack with melee
 * 2 - Capture
 * 3 - Defend
 * 4 - Flee
 *
 * QueueItem takes the string of the item to be used as an argument
 *
 * QueueDragon has two overloaded options:
 *      QueueDragon(int code): Use if no spells are being cast and the
 *                             Dragon is just attacking
 *
 *      QueueDragon(string s): Use if a spell is being cast
 *
 * The codes for the Dragon are as follows:
 * 0 - No action (default)
 * 1 - Attack with melee
 * 2 - Attack with breath weapon
 * 3 - Use spell (Do not set manually, call QueueDragon(string s))
 * 4 - Defend
 *
 * Once all the actions have been queued up, call TakeRound()
 * to execute all the actions
 *
 * The variables will be reset automatically after the round
 * and must be queued again on the next round
 ***************************************************************/

public class CombatManager : MonoBehaviour
{
	public enum PlayerAction
	{
		None,
		Melee,
		Capture,
		Defend,
		Flee
	}

	public enum DragonAction
	{
		None,
		Attack,
		Breath,
		Spell,
		Defend,
        Switch
	}

    public Sprite grasslandBkgd, cavesBkgd, skyBkgd, bossBkgd;
    public SpriteRenderer bkgdRenderer;


    public EntityData Player;
    public EntityData Dragon;
    public EntityData Enemy;

    public CombatUIManager CUI;

	private PlayerAction player_action;
	private DragonAction dragon_action;
    private string item;
    private bool SuccessUse;
    private int spellIndex;
    private int targetID;
    private int switchCode;
    private bool captured;

    private int RoundCounter = 0;

    //private List<string> listOfDragonTypes = new List<string>() {
    //    "Basic Drakling", "Earthen Drakling", "Flame Drakling", "Tidal Drakling",
    //    "Basic Wyrm", "Earthen Wyrm", "Flame Wyrm", "Tidal Wyrm",
    //    "Basic Serpent", "Earthen Serpent", "Flame Serpent", "Tidal Serpent"};
    //private int numberOfDragonTypes;


    // Use this for initialization
    void Awake()
    {
        GlobalFlags.GetCombatManagerFlags(out Player, out Dragon, out Enemy);

        if(GlobalFlags.GetBossFlag())
            GameObject.Find("Enemy").GetComponent<Transform>().position = new Vector3(-4.0f, 1.0f, 0f);
        else
            GameObject.Find("Enemy").GetComponent<Transform>().position = new Vector3(-6.0f, 0.6f, 0f);
        GameObject.Find("Enemy").GetComponent<SpriteRenderer>().sprite = Enemy.battleSprite;

        string sceneName = GlobalFlags.GetCurrentOverworldScene();

        if (sceneName == "Overworld") bkgdRenderer.sprite = grasslandBkgd;
        if (sceneName == "Overworld - West") bkgdRenderer.sprite = skyBkgd;
        if (sceneName == "Caves") bkgdRenderer.sprite = cavesBkgd;
        if (sceneName == "Boss") bkgdRenderer.sprite = bossBkgd; 
    }

	void Start ()
    {
        player_action = 0;
        dragon_action = 0;
        item = string.Empty;
        SuccessUse = false;
        captured = false;
        Enemy.Heal(Enemy.GetMaxHealth());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // The main function used to resolve a round of combat
    public void TakeRound()
    {
        CUI.DisplayLine("");

        // Apply defense modifiers first
        if (player_action == PlayerAction.Defend)
        {
            Player.ModifyDefense(4);
        }

        if (dragon_action == DragonAction.Defend)
        {
            Player.ModifyDefense(4);
        }

        // If Player Speed > Enemy Speed and Dragon Speed, Player goes first
		if (Player.GetSpeed() >= Enemy.GetSpeed() && Player.GetSpeed() >= Dragon.GetSpeed())
        {
            PlayerTurn();

            // If Enemy not dead
			if (!Enemy.IsDead())
            {
                //if a dragon exists and is not dead, it gets a turn
                if(Dragon != null && !Dragon.IsDead())
                {
                    //if it's faster than the enemy
                    if(Dragon.GetSpeed() >= Enemy.GetSpeed())
                    {
                        DragonTurn();
                        // If Enemy still not dead and not caputured
                        if (!Enemy.IsDead() || !captured)
                        {
                            EnemyTurn();
                        }
                    }
                    //else if enemy is faster than dragon
                    else
                    {
                        // If Enemy still not dead and not caputured
                        if (!Enemy.IsDead() || !captured)
                        {
                            EnemyTurn();
                        }
                        //if dragon is not dead, dragon's turn
                        if(!Dragon.IsDead())
                            DragonTurn();
                    }
                }
                //else if no usable dragonn
                else
                {
                    // If Enemy still not dead and not caputured
				    if (!Enemy.IsDead() || !captured)
                    {
                        EnemyTurn();
                    }
                }
            }
        }
        //Else if dragon is fastest
        else if(Dragon != null && !Dragon.IsDead() && Dragon.GetSpeed() >= Enemy.GetSpeed())
        {
            DragonTurn();

            // If Enemy not dead and player not dead
            if (!Enemy.IsDead() && !Player.IsDead())
            {
                //if player's faster than the enemy
                if (Player.GetSpeed() >= Enemy.GetSpeed())
                {
                    PlayerTurn();
                    // If Enemy still not dead and not caputured
                    if (!Enemy.IsDead() || !captured)
                    {
                        EnemyTurn();
                    }
                }
                //else if enemy is faster than player
                else
                {
                    // If Enemy still not dead and not caputured
                    if (!Enemy.IsDead() || !captured)
                    {
                        EnemyTurn();
                    }
                    //if player is not dead, player's turn
                    if (!Player.IsDead())
                        PlayerTurn();
                }
            }
        }
        //Else if enemy is fastest
        else
        {
            EnemyTurn();
            //If Player not dead
            if (!Player.IsDead())
            {
                //if a dragon exists and is not dead, it gets a turn
                if (Dragon != null && !Dragon.IsDead())
                {
                    //if dragon's faster than the player
                    if (Dragon.GetSpeed() >= Player.GetSpeed())
                    {
                        DragonTurn();
                        //if enemy and player not dead, player's turn
                        if (!Enemy.IsDead() && !Player.IsDead())
                        {
                            PlayerTurn();
                        }
                    }
                    //else if player is faster than dragon
                    else
                    {
                        PlayerTurn();
                        //if dragon is not dead and enemy is not caputured or dead, dragon's turn
                        if (!Dragon.IsDead() && !Enemy.IsDead() && !captured)
                            DragonTurn();
                    }
                }
                //else if no usable dragonn
                else
                    PlayerTurn();
            }
        }

        // Reset variables at the end of the round
        player_action = PlayerAction.None;
		dragon_action = DragonAction.None;
        item = string.Empty;
        SuccessUse = false;

        // Reset any modifiers that occurred this round
        Player.ResetStats();
        Dragon.ResetStats();
        Enemy.ResetStats();

        // Reset queue status
        CUI.ResetQueue();

        CUI.UpdateSliders(Player.GetCurrentHealth(), Dragon.GetCurrentHealth(), Dragon.GetCurrentMana(), Enemy.GetCurrentHealth());

        // If Player dead
		if (Player.IsDead() || Player.AllDragonsDead())
        {
            // Game over
            CUI.DisplayLine("Game over");
            CUI.Wait();
        }

        // If Dragon dead
		if (Dragon.IsDead())
        {
            GameObject.Find("Dragon").GetComponent<SpriteRenderer>().enabled = false;
            //if the player has dragons left with > 0 HP, send one out
            if (!Player.AllDragonsDead())
                CUI.SendOutDragon();
        }

        // If Enemy dead
		if (Enemy.IsDead())
        {
            // Award experience and gold
            //float gold = Enemy.GetGold();
            float gold = Random.Range(1, 25);
            float exp = 100;

            InventoryCanvasController.instance.InsertReward();

			Player.AddGold(gold);
			Player.GainExperience(100);
			Dragon.GainExperience(100);

            CUI.DisplayLine("");
            CUI.DisplayLine("Enemy defeated!");
            CUI.DisplayLine("Gained " + gold.ToString() + " gold!");
            CUI.DisplayLine("Gained " + exp.ToString() + " XP!");
            CUI.DisplayLine("Press any key to continue...");

            CUI.Wait();
        }

        RoundCounter++;
    }

    public void EndCombat()
    {
        SoundManager.Instance.StopMusic();
        if (!Player.IsDead() && !Player.AllDragonsDead())
        {
            //Was this the final boss???
            if (GlobalFlags.GetBossFlag() == true) {

                GameManager.instance.PushState(StateManagement.GameStateType.Credits);

            } else {

                GameManager.instance.PopState();

            }
        }
        else
        {
            GameManager.instance.PopState();
            GameManager.instance.PopState();
        }
    }

    // Used to queue Player action
    // code specifies what will be done
    public void QueuePlayer(int code)
    {
        if(item == string.Empty)
        {
            player_action = (PlayerAction)code;
        }

        // Since the UI does not prevent the player from attacking multiple times 
        // then there needs to be a check for TakeRound based on item.

        // Since an attack is being taken, no item can be used
        // item = string.Empty;

        if (player_action == PlayerAction.Flee)
        {
            GameManager.instance.PopState();
        }

        switch (code)
        {
            case (int) PlayerAction.Melee:
                CUI.DisplayLine(Player.GetName() + " prepares to attack.");
                break;
            
            case (int) PlayerAction.Capture:
                CUI.DisplayLine(Player.GetName() + " prepares to capture the dragon.");

                dragon_action = DragonAction.None;

                CUI.QueuePlayer();
                CUI.QueueDragon();

                TakeRound();
                return;
            
            case (int) PlayerAction.Defend:
                CUI.DisplayLine(Player.GetName() + " braces against attacks.");
                break;
            
            case (int) PlayerAction.Flee:
                CUI.DisplayLine(Player.GetName() + " prepares to flee.");
                break;
        }

        CUI.QueuePlayer();

		if((Dragon == null || (!Dragon.IsDead() && dragon_action != DragonAction.None)) && item == string.Empty)
		{
			TakeRound();
		}
    }

    // Used to queue Dragon action
    // code specifies what will be done
    public void QueueDragon(int code)
    {
        if (!Dragon.IsDead() && Dragon != null)
        {
            dragon_action = (DragonAction)code;
        }
        else
        {
            dragon_action = DragonAction.None;
        }

        switch (code)
        {
            case (int) DragonAction.Attack:
                CUI.DisplayLine(Dragon.GetName() + " prepares to attack.");
                break;
            
            case (int) DragonAction.Breath:
                CUI.DisplayLine(Dragon.GetName() + " prepares to use its breath weapon.");
                break;
            
            case (int) DragonAction.Spell:
                CUI.DisplayLine(Dragon.GetName() + " readies a spell.");
                break;

            case (int) DragonAction.Defend:
                CUI.DisplayLine(Dragon.GetName() + " braces against attacks.");
                break;

            case (int)DragonAction.Switch:
                CUI.DisplayLine(Dragon.GetName() + " switches with an ally.");
                break;
        }

        CUI.QueueDragon();

		if(player_action != PlayerAction.None || item != string.Empty)
		{
			TakeRound();
		}
    }

    // Overloaded version of QueueDragon if a spell is being cast
    public void QueueDragonSpell(int spell, int target)
    {
        spellIndex = spell;
        targetID = target;
		QueueDragon(3);
    }

    public bool DragonSpellCastable(int index)
    {
        if (Dragon.Spells[index].GetManaCost() > Dragon.GetCurrentMana())
            return false;
        else
            return true;
    }

    public void QueueDragonSwitch(int dragonCode)
    {
        switchCode = dragonCode;
        QueueDragon((int)DragonAction.Switch);
    }

    public bool IsDragonAlive(int index)
    {
        //returns the opposite because isDead returns true if dead, but this function returns true if alive
        return !Player.DragonList[index].IsDead();
    }

    /// <summary>
    /// Method that queues an item when it's used in combat
    /// </summary>
    public void QueueItem()
    {
        //if player has not selected different action and item was not used
        if (player_action == PlayerAction.None && !SuccessUse)
        {
            item = Player.ItemUsed;
            SuccessUse = InventoryCanvasController.instance.InventoryStartUp.UseItem(item);
            InventoryCanvasController.instance.InventoryStartUp.CreateInventorySlot();
        }

        else
        {
            TextControl TempText = InventoryCanvasController.instance.ItemDialog.GetComponent<TextControl>();
            TempText.ItemDialog.text = "You have already selected another action!";
        }

        if (!SuccessUse)
        {
            Player.ItemUsed = string.Empty;
            item = Player.ItemUsed;
        }

        CUI.UpdatePlayerSlider(Player.GetCurrentHealth());

        // Since an item is being used, no other Player action can be taken
        //player_action = 0;

        if (!Dragon.IsDead() && dragon_action != DragonAction.None && player_action == PlayerAction.None && SuccessUse)
        {
            TakeRound();
        }
    }

    void PlayerTurn()
    {
        // Check Player action code
        // If code != 0, perform the action
		if(player_action != PlayerAction.None)
        {
            switch(player_action)
            {
                // 1: Attack with melee
				case PlayerAction.Melee:
					Attack(Player, Enemy);
                    break;

                // 2: Capture
				case PlayerAction.Capture:
                    Capture();
                    break;

                // 3: Defend
				case PlayerAction.Defend:
                    break;

                // 4: Run
				case PlayerAction.Flee:
                    GameManager.instance.PopState();
                    break;
            }
        }
        // Else
        else
        {
            // If code == 0, do nothing
        }
    }

    void DragonTurn()
    {
        // Check Dragon action code
        // If code != 0, perform the action
		if (dragon_action != DragonAction.None)
        {
            switch (dragon_action)
            {
                // 1: Attack with melee
				case DragonAction.Attack:
					Attack(Dragon, Enemy);
                    break;

                // 2: Attack with breath weapon
                case DragonAction.Breath:
                    break;

                // 3: Spell
                case DragonAction.Spell: 
                    UseSpell(Dragon, spellIndex, ChooseTarget(targetID));
                    break;

                // 4: Defend
                case DragonAction.Defend:
                    break;

                // 5: Switch
                case DragonAction.Switch:
                    SwitchDragon(switchCode);
                    break;
            }
        }
    }

    void EnemyTurn()
    {
        //if the enemy has been caputured, it shouldn't do anything
        if(!captured)
        {
            // If Enemy health > 30%, it will attack
            if (Enemy.GetCurrentHealth() > (Enemy.GetMaxHealth() * 0.3))
            {
                //50% chance to attack player, 50% chance to attack dragon
                if (Random.Range(0, 100) < 50)
                    Attack(Enemy, Player);
                else
                    Attack(Enemy, Dragon);
            }
            //If Enemy health <= 30% and healing magic available, it has an 80% chance of healing
            else if (Enemy.GetCurrentHealth() <= (Enemy.GetMaxHealth() * 0.3))
            {
                bool willHeal = false;

                //check each spell in Enemy's spell list
                for(int i = 0; i < Enemy.numSpells; i++)
                {
                    //if a healing spell is found and there is enough mana
                    if (Enemy.Spells[i].GetSpellClass() == SpellData.SpellClass.HealHealth && Enemy.Spells[i].GetManaCost() <= Enemy.GetCurrentMana())
                    {  
                        //80% chance enemy will heal, otherwise it will attack
                        if(Random.Range(0, 100) < 80)
                        {
                            willHeal = true;
                            UseSpell(Enemy, i, Enemy);
                        }
                    }
                }
                //If high health or no healing available, enemy will attack
                if (!willHeal)
                {
                    //50% chance to attack player, 50% chance to attack dragon
                    if (Random.Range(0, 100) < 50)
                        Attack(Enemy, Player);
                    else
                        Attack(Enemy, Dragon);
                }
            }
        }
    }

    void Attack(EntityData attacker, EntityData defender)
    {
        //90% chance for attack to connect, 10% chance to miss
        if(Random.Range(0, 100) < 90)
        {
            float damage = Mathf.Max(Random.Range(2, 4) + attacker.GetOffense() - defender.GetDefense(), 1.0f);
		    defender.TakeDamage(damage);
            if(attacker == Enemy)
                CUI.DisplayLine("Enemy " + attacker.GetName() + " dealt " + damage.ToString() + " damage to " + defender.GetName() + ".");
            else if(defender == Enemy)
                CUI.DisplayLine(attacker.GetName() + " dealt " + damage.ToString() + " damage to " + "Enemy " + defender.GetName() + ".");
            else
                CUI.DisplayLine(attacker.GetName() + " dealt " + damage + " damage to " + defender.GetName() + ".");
        }
        else
        {
            if (attacker == Enemy)
                CUI.DisplayLine("Enemy " + attacker.GetName() + "'s attack missed!");
            else
                CUI.DisplayLine(attacker.GetName() + "'s attack missed!");
        }
    }

    EntityData ChooseTarget(int targetID)
    {
        EntityData target = null;

        switch(targetID)
        {
            //if target is player character
            case 0:
                target = Player;
                break;
            //if target is dragon
            case 1:
                target = Dragon;
                break;
            //if target is enemy
            case 2:
                target = Enemy;
                break;
        }

        return target;
    }

    void UseSpell(EntityData caster, int spellIndex, EntityData target)
    {
        //choose target

        switch (caster.Spells[spellIndex].GetSpellClass()) //gets the type of spell being cast by the caster
        {
            // 1: Offensive Spell
            case SpellData.SpellClass.Offense:
                target.ModifyOffense(caster.Spells[spellIndex].GetSpellPower());
                break;

            // 2: Defensive Spell
            case SpellData.SpellClass.Defense:
                target.ModifyDefense(caster.Spells[spellIndex].GetSpellPower());
                break;

            // 3: Heal Health
            case SpellData.SpellClass.HealHealth:
                target.Heal(caster.Spells[spellIndex].GetSpellPower());
                break;

            // 4: Restore Mana
            case SpellData.SpellClass.RestoreMana:
                target.RestoreMana(caster.Spells[spellIndex].GetSpellPower());
                break;
        }

        caster.UseSpell(caster.Spells[spellIndex].GetManaCost());

        if(caster == Enemy)
            CUI.DisplayLine("Enemy " + caster.GetName() + " cast " + caster.Spells[spellIndex].GetSpellName());
        else
            CUI.DisplayLine(caster.GetName() + " cast " + caster.Spells[spellIndex].GetSpellName());
    }

    public void SwitchDragon(int newDragon)
    {
        Player.ChangeActiveDragon(newDragon);
        Dragon = Player.GetActiveDragon();
        GameObject.Find("Dragon").GetComponent<SpriteRenderer>().sprite = Dragon.battleSprite;
        GameObject.Find("Dragon").GetComponent<SpriteRenderer>().enabled = true;
    }

    public void TakeParameters(EntityData player, EntityData dragon, EntityData enemy)
	{
		Player = player;
		Dragon = dragon;
		Enemy = enemy;

        Enemy.Heal(Enemy.GetMaxHealth());
        Enemy.Dead = false;
	}

    public float[] GetPlayerInfo()
    {
        float[] playerInfo = new float[2];

        playerInfo[0] = Player.GetMaxHealth();
        playerInfo[1] = Player.GetCurrentHealth();

        return playerInfo;
    }

    public float[] GetDragonInfo()
    {
        float[] dragonInfo = new float[4];

        if(Dragon != null)
        {
        dragonInfo[0] = Dragon.GetMaxHealth();
        dragonInfo[1] = Dragon.GetCurrentHealth();
        dragonInfo[2] = Dragon.GetMaxMana();
        dragonInfo[3] = Dragon.GetCurrentMana();
        }

        return dragonInfo;
    }

    public string[] GetDragonSpells()
    {
        string[] spells = new string[4];

        if(Dragon != null)
        {
        for(int i = 0; i < 4; i++)
            spells[i] = Dragon.Spells[i].GetSpellName();
        }

        return spells;
    }

    public string[] GetDragonNames()
    {
        string[] dragonNames = new string[4];

        for (int i = 0; i < 4; i++)
            dragonNames[i] = Player.GetDragonNameFromList(i);

        return dragonNames;
    }

    public float[] GetEnemyInfo()
    {
        float[] enemyInfo = new float[2];

        enemyInfo[0] = Enemy.GetMaxHealth();
        enemyInfo[1] = Enemy.GetCurrentHealth();

        return enemyInfo;
    }

    public void Capture()
    {
        if (Random.Range(0, Enemy.GetMaxHealth()) < Enemy.GetCurrentHealth())
        {
            CUI.DisplayLine("Capture failed!");
        }
        else
        {
            dragon_action = DragonAction.None;
            captured = true;
            CUI.CaptureDragon();
            CUI.DisplayLine(Enemy.GetName() + " captured!");
            GameObject.Find("Enemy").GetComponent<SpriteRenderer>().enabled = false;
            if(Player.GetNumDragons() < 4)
            {
                Player.AddDragon(Enemy.GetName(), -1);
                CUI.Wait();
            }
            else
            {
                CUI.ReplaceDragon();
            }
        }
    }

    public void ReplaceDragon(int index)
    {
        if(index < 4)
            Player.AddDragon(Enemy.GetName(), index);
        EndCombat();
    }

    public void PlayerCheat(float hp)
    {
        Player.SetCurrentHealth(hp);
    }

    public void DragonCheat(float hp, float mana)
    {
        Dragon.SetCurrentHealth(hp);
        Dragon.SetCurrentMana(mana);
    }

    public void EnemyCheat(float hp)
    {
        Enemy.SetCurrentHealth(hp);
    }
}
