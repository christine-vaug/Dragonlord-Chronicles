using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonlordChroniclesDatabase;

public static class GlobalFlags {

    static GlobalFlags () {

        firstTime = true;

    }

    //******************************************//
    //          Shopkeeper      Flags           //
    //******************************************//

    public static ShopKeeperData ShopKeeperData;
    public static InventorySystem.Inventory PlayerInventory;
    public static EntityData PlayerData;

    //******************************************//
    //          SceneManagement Flags           //
    //******************************************//

    private static string CurrentOverworldScene;
    private static string CurrentBattleScene;
    private static Vector2 PlayerPosition;

    public static string GetCurrentOverworldScene() {

        return CurrentOverworldScene;
    }

    public static void SetCurrentOverworldScene(string name) {

        CurrentOverworldScene = name;
    }

    public static string GetCurrentBattleScene () {

        return CurrentBattleScene;
    }

    public static void SetCurrentBattleScene (string name) {

        CurrentBattleScene = name;
    }

    public static void SetPlayerPosition (Vector2 pos) {

        PlayerPosition = pos;
    }

    public static Vector2 GetPlayerPosition () {

        return PlayerPosition;
    }

    //******************************************//
    //          CombatManager Flags             //
    //******************************************//

    private static EntityData Player;
    private static EntityData Dragon;
    private static EntityData Enemy;
    private static bool isBoss;

    //NOTE: I am using functions to make it easier to track when flags are getting set/get

    public static void SetCombatManagerFlags (EntityData player, EntityData dragon, EntityData enemy) {

        Player = player;
        Dragon = dragon;
        Enemy = enemy;
    }

    public static void GetCombatManagerFlags (out EntityData player, out EntityData dragon, out EntityData enemy) {

        player = Player;
        dragon = Dragon;
        enemy = Enemy;
    }

    public static void GetDragonFlag(out EntityData dragon)
    {
        dragon = Dragon;
    }

    public static void SetBossFlag(bool boss)
    {
        isBoss = boss;
    }

    public static bool GetBossFlag()
    {
        return isBoss;
    }


    //******************************************//
    //          SERIALIZATION Flags             //
    //******************************************//
    static bool firstTime;

    public static bool GetFirstTimeFlag () {
        return firstTime;
    }

    public static void SetFirstTimeFlag (bool value) {
        firstTime = value;
    }
}
