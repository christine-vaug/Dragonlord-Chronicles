using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;
using DragonlordChroniclesDatabase;

public class EnemyBehavior : MonoBehaviour
{
    /*private enum Personality
    {
        Aggressive,
        Neutral,
        Timid
    }*/

    bool canHeal = false;

    public void EnemyAction(EntityData Player, EntityData Dragon, EntityData Enemy)
    {
        // If Enemy health > 30%
        if (Enemy.GetCurrentHealth() > (Enemy.GetMaxHealth() * 0.3))
        {
            EnemyAttack(Player, Dragon, Enemy);
            return;
        }
        // Else if Enemy health <= 30% and healing magic available, heal
        else if (Enemy.GetCurrentHealth() <= (Enemy.GetMaxHealth() * 0.3))
        {
            canHeal = false;

            //////////////////////////////////Enemy.Spells.Where(spell => spell.GetSpellClass() == SpellData.SpellClass.HealHealth); - leave this for now

            //check each spell in Enemy's spell list
            foreach (SpellData spell in Enemy.Spells)
            {
                //if a healing spell is found and there is enough mana, canHeal == true
                if (spell.GetSpellClass() == SpellData.SpellClass.HealHealth && spell.GetManaCost() <= Enemy.GetCurrentMana())
                {
                    canHeal = true;
                    //deduct mana cost
                    Enemy.UseSpell(spell.GetManaCost());
                    //heal hp
                    Enemy.Heal(spell.GetSpellPower());
                    //enemy has healed, so end enemy turn
                    return;
                }      
            }
            // Else (low health and no healing available)
            if(!canHeal)
            {
                // 10% for Enemy to flee and end encounter
                // 90% for Enemy to attack normally
                if (Random.Range(0, 100) >= 90)
                {
                    // Flee
                    GameManager.instance.PopState();
                }
                else
                {
                    EnemyAttack(Player, Dragon, Enemy);
                    return;
                }
            }
        }
    }

    public void EnemyAttack(EntityData Player, EntityData Dragon, EntityData Enemy)
    {
        //45% chance to attack player, 45% chance to attack dragon, 10% chance to miss
        int rand = Random.Range(0, 100);
        if (rand < 45)
        {
            Player.TakeDamage(Mathf.Max((Random.Range(1, 10) * Enemy.GetOffense() * 0.1f) - (0.1f * Player.GetDefense()), 1.0f));
        }
        else if (rand >= 45 && rand < 90)
        {
            Dragon.TakeDamage(Mathf.Max((Random.Range(1, 10) * Enemy.GetOffense() * 0.1f) - (0.1f * Player.GetDefense()), 1.0f));
        }
        else
        {
            //miss
            return;
        }
    }

    /*public void EnemyHeal(EntityData Player, EntityData Dragon, EntityData Enemy)
    {

    }*/
}
