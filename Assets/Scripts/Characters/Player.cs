using UnityEngine;
using UnityEditor;
using DragonlordChroniclesDatabase;
using InventorySystem;

/// <summary>
/// This class represents the behavior of the player and its related data. Extends Entity.
/// </summary>
public class Player : Entity {

    private enum PlayerState {
        Exploring = 1,
        Talking = 2,
        Checking = 3,
        Inventory = 4,
        Battling = 5,
        Shopping = 6,
    }


    public float moveSpeed;

    public EntityData P;
    public EntityData D;
    //public EntityData E;

    private Animator2D animator;
    private Vector2Int lookDirection;
    private PlayerState state;

    private static float enterBattleDelay;

    // Use this for initialization
    void Awake () {

        Vector2 pos = GlobalFlags.GetPlayerPosition();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        P = (EntityData)entityData;
        //if (P.GetNumDragons() == 0)
        //    P.AddDragon("Basic Drakling", -1);

        lookDirection = new Vector2Int(0, 0);
        state = PlayerState.Exploring;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator2D>();

    }

    void Start () {

        GlobalFlags.PlayerData = (EntityData)entityData;

        if (GlobalFlags.PlayerData.CurrentHealth == 0) GlobalFlags.PlayerData.CurrentHealth = GlobalFlags.PlayerData.MaxHealth;
    }

    // Update is called once per frame
    void Update () {
		
        if (enterBattleDelay >= 0) {
            enterBattleDelay -= Time.deltaTime;
        }
	}

    void LateUpdate () {
        
        if (state == PlayerState.Exploring) {

            CheckActionButton();
            MovePlayer();

        } else {

            rigidbody.velocity = Vector2.zero;
        }

        if (state == PlayerState.Talking && 
            DialogueCanvasController.instance.IsFinished() == true && (ShopkeeperCanvasController.Instance == null || ShopkeeperCanvasController.Instance.IsFinished() == true)) {

            state = PlayerState.Exploring;
        }

        CheckInventory();
    }

    void MovePlayer () {


        Vector2 desiredMove = GetDesiredMove();
        

        if (desiredMove.x > 0) {
            animator.SetAnimationClip("MoveSide", flip: true);
        }

        if (desiredMove.x < 0) {
            animator.SetAnimationClip("MoveSide");
        }

        if (desiredMove.y > 0) {
            animator.SetAnimationClip("MoveUp");
        }

        if (desiredMove.y < 0) {
            animator.SetAnimationClip("MoveDown");
        }

        if (desiredMove == Vector2.zero) {
            animator.StopAnimations();
        }

        rigidbody.velocity = desiredMove * moveSpeed;
    }

    Vector2 GetDesiredMove () {

        Vector2 move = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            lookDirection.x = 0;
            lookDirection.y = 1;
            move.y = 1;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            lookDirection.x = 0;
            lookDirection.y = -1;
            move.y = -1;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            lookDirection.x = 1;
            lookDirection.y = 0;
            move.x = 1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            lookDirection.x = -1;
            lookDirection.y = 0;
            move.x = -1;
        }

        return move.normalized;
    }

    void CheckActionButton () {
    
        if (Input.GetKeyDown(KeyCode.Space)) {

            RaycastHit2D result = Physics2D.Raycast(transform.position, lookDirection, 1f, LayerMask.GetMask("TalkingCharacter"));
            if (result) {

                TalkingCharacter coll = result.collider.gameObject.GetComponent<TalkingCharacter>();
                ShopCharacter shop = result.collider.gameObject.GetComponent<ShopCharacter>();

                if (shop != null) {

                    GlobalFlags.PlayerData = (EntityData)entityData;
                    GlobalFlags.ShopKeeperData = shop.shopData;

                }

                if (coll != null) {

                    DialogueCanvasController.instance.SetDialogueTree(coll.DialogueTreeName);
                    DialogueCanvasController.instance.Activate();

                    animator.StopAnimations();

                    state = PlayerState.Talking;
                }


            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            state = PlayerState.Exploring;
        }
    }

    /// <summary>
    /// Method that checks whether I key was hit to open or close inventory
    /// </summary>
    void CheckInventory()
    {
        if (Input.GetKeyDown(KeyCode.I)){

            if (state == PlayerState.Exploring) {

                state = PlayerState.Inventory;
                InventoryCanvasController.instance.ExitButton.gameObject.SetActive(false);
                InventoryCanvasController.instance.InventoryStartUp.CreateInventorySlot();
                InventoryCanvasController.instance.OverWorld = true;
                InventoryCanvasController.instance.Activate();
            }

            else if (state == PlayerState.Inventory) {

                state = PlayerState.Exploring;
                InventoryCanvasController.instance.Deactivate();
                InventoryCanvasController.instance.ExitButton.gameObject.SetActive(true);
                InventoryCanvasController.instance.OverWorld = false;
            }
        }
    } 

    void OnCollisionStay2D (Collision2D coll) {

        if (coll.gameObject.tag == "Boss") {

            P = (EntityData)entityData;
            if (P.GetNumDragons() > 0)
                D = ((EntityData)entityData).GetActiveDragon();
            else
                D = null;
            EntityData E = (EntityData)Resources.Load("ScriptableObjects/Dragons/Boss");

            GlobalFlags.SetPlayerPosition(new Vector2(transform.position.x, transform.position.y));
            GlobalFlags.SetCombatManagerFlags(P, D, E);

            GlobalFlags.SetBossFlag(true);
        }


        if ((coll.gameObject.tag == "Enemy" && enterBattleDelay <= 0f)) {

            P = (EntityData)entityData;
            if (P.GetNumDragons() > 0)
                D = ((EntityData)entityData).GetActiveDragon();
            else
                D = null;
            EntityData E = (EntityData)coll.gameObject.GetComponent<Entity>().entityData;

            GlobalFlags.SetPlayerPosition(new Vector2(transform.position.x, transform.position.y));
            GlobalFlags.SetCombatManagerFlags(P, D, E);

            GlobalFlags.SetBossFlag(false);

            state = PlayerState.Battling;

            SoundManager.Instance.StopMusic();

            GameManager.instance.PushState(StateManagement.GameStateType.Battle);
        }



        //if (coll.gameObject.tag == "Enemy" && enterBattleDelay <= 0f) {

        //    P = (EntityData)entityData;
        //    if (P.GetNumDragons() > 0)
        //        D = ((EntityData)entityData).GetActiveDragon();
        //    else
        //        D = null;

        //    EntityData E = (EntityData)coll.gameObject.GetComponent<Entity>().entityData;

        //    GlobalFlags.SetPlayerPosition(new Vector2(transform.position.x, transform.position.y));
        //    GlobalFlags.SetCombatManagerFlags(P, D, E);
        //    GlobalFlags.SetBossFlag(false);

        //    state = PlayerState.Battling;

        //    SoundManager.Instance.StopMusic();

        //    GameManager.instance.PushState(StateManagement.GameStateType.Battle);
        //}
    }

    public static void SetEnterBattleDelayTimer (float delay = 1f) {

        enterBattleDelay = delay;
    }
    
}
