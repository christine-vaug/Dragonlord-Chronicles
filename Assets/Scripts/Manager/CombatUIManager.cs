using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using InventorySystem;

public class CombatUIManager : MonoBehaviour
{
    public CombatManager CM;
    public EventSystem ES;

    public GameObject MainPanel;
    public GameObject PlayerPanel;
    public GameObject DragonPanel;
    public GameObject DragonSelectPanel;
    public GameObject DragonSpellsPanel;
    public GameObject TargetSelectPanel;

    public Button PlayerButton;
    public Button DragonButton;
    public Button CaptureButton;
    public Button FleeButton;

    public Button PlayerAttack;
    public Button PlayerInventory;
    public Button PlayerDefend;
    public Button PlayerBack;

    public Button DragonAttack;
    public Button DragonMagic;
    public Button DragonDefend;
    public Button DragonSwitch;
    public Button DragonBack;

    public Button SpellButton1;
    public Button SpellButton2;
    public Button SpellButton3;
    public Button SpellButton4;
    private string[] SpellNames = new string[4];
    //private bool useSpell;

    public Button DragonSelectButton1;
    public Button DragonSelectButton2;
    public Button DragonSelectButton3;
    public Button DragonSelectButton4;
    private string[] DragonNames = new string[4];

    public enum DragonSelectType
    {
        SwappingDragon, //if 0, dragon select is being used to choose which dragon to swap in
        ReleasingDragon, //if 1, dragon select is being used to choose which dragon to release
        SendingOutDragon //if 2, dragon select is being used to choose which dragon to send out after one has died
    }

    private DragonSelectType dst;

    public Button TargetSelectPlayer;
    public Button TargetSelectDragon;
    public Button TargetSelectEnemy;

    public Slider PlayerSlider;
    public Slider DragonSlider;
    public Slider DragonManaSlider;
    public Slider EnemySlider;

    public GameObject InfoTextPanel;
    private const int TextQueueLength = 4;
    private string[] TextQueue = new string[TextQueueLength];

    private int DragonCode;
    private int SpellCode;
    private int TargetCode;

    private UnityEvent KeyPress = new UnityEvent();

    private bool IsHalted = false;
    private bool ItemTextBox = false;

    private bool PlayerQueued = false;
    private bool DragonQueued = false;
    private bool DragonCaptured = false;

    //private bool IsBossBattle;

    // Code codes for buttons
    private byte BaseColor = 0x80;
    private byte HighlightedColor = 0xC0;
    private byte ClickedColor = 0x60;

    // Use this for initialization
    void Start()
    {
        ColorInit();
        ES.SetSelectedGameObject(MainPanel);

        PlayerButton.onClick.AddListener(() => PlayerButtonClick());
        DragonButton.onClick.AddListener(() => DragonButtonClick());
        CaptureButton.onClick.AddListener(CaptureButtonClick);
        FleeButton.onClick.AddListener(FleeButtonClick);

        PlayerAttack.onClick.AddListener(PlayerAttackClick);

        PlayerInventory.onClick.AddListener(PlayerInventoryClick);
        InventoryCanvasController.instance.UseButton.onClick.AddListener(PlayerInventoryUseClick);
        InventoryCanvasController.instance.CancelButton.onClick.AddListener(PlayerInventoryCancelClick);

        PlayerDefend.onClick.AddListener(PlayerDefendClick);
        PlayerBack.onClick.AddListener(() => PlayerBackClick());

        DragonAttack.onClick.AddListener(DragonAttackClick);
        DragonMagic.onClick.AddListener(DragonMagicClick);
        DragonDefend.onClick.AddListener(DragonDefendClick);
        DragonSwitch.onClick.AddListener(DragonSwitchClick);
        DragonBack.onClick.AddListener(() => DragonBackClick());

        DragonSelectButton1.onClick.AddListener(() => DragonSelect(0, dst));
        DragonSelectButton2.onClick.AddListener(() => DragonSelect(1, dst));
        DragonSelectButton3.onClick.AddListener(() => DragonSelect(2, dst));
        DragonSelectButton4.onClick.AddListener(() => DragonSelect(3, dst));

        SpellButton1.onClick.AddListener(() => SpellSelect(0));
        SpellButton2.onClick.AddListener(() => SpellSelect(1));
        SpellButton3.onClick.AddListener(() => SpellSelect(2));
        SpellButton4.onClick.AddListener(() => SpellSelect(3));

        TargetSelectPlayer.onClick.AddListener(() => TargetSelect(0));
        TargetSelectDragon.onClick.AddListener(() => TargetSelect(1));
        TargetSelectEnemy.onClick.AddListener(() => TargetSelect(2));

        SpellNames = CM.GetDragonSpells();
        SpellButton1.GetComponentInChildren<Text>().text = SpellNames[0];
        SpellButton2.GetComponentInChildren<Text>().text = SpellNames[1];
        SpellButton3.GetComponentInChildren<Text>().text = SpellNames[2];
        SpellButton4.GetComponentInChildren<Text>().text = SpellNames[3];

        DragonNames = CM.GetDragonNames();
        DragonSelectButton1.GetComponentInChildren<Text>().text = DragonNames[0];
        DragonSelectButton2.GetComponentInChildren<Text>().text = DragonNames[1];
        DragonSelectButton3.GetComponentInChildren<Text>().text = DragonNames[2];
        DragonSelectButton4.GetComponentInChildren<Text>().text = DragonNames[3];

        PlayerSlider.onValueChanged.AddListener(PlayerCheat);
        DragonSlider.onValueChanged.AddListener(DragonCheat);
        DragonManaSlider.onValueChanged.AddListener(DragonCheat);
        EnemySlider.onValueChanged.AddListener(EnemyCheat);

        PlayerSlider.interactable = false;
        DragonSlider.interactable = false;
        DragonManaSlider.interactable = false;
        EnemySlider.interactable = false;

        ShowMainMenu(true);

        GetSliderInfo();

        //IsBossBattle = CM.IsFightingBoss();

        for (int i = 0; i < TextQueueLength; i++)
        {
            TextQueue[i] = "";
        }

        if (GlobalFlags.GetBossFlag())
            DisplayLine("This is your final challenge!");
        else
            DisplayLine("An enemy appeared!");
	}

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (IsHalted)
            {
                ShowMainMenu(true);
                CM.EndCombat();
            }
            else
            {
                GeneralBack();
            }

            if (ItemTextBox)
            {
                InventoryCanvasController.instance.ItemDialog.gameObject.SetActive(false);
                InventoryCanvasController.instance.Deactivate();
                ShowPlayerPanel(false);
                ShowMainMenu(true);
                ItemTextBox = false;
                PlayerButton.interactable = false;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            PlayerSlider.interactable = true;
            DragonSlider.interactable = true;
            DragonManaSlider.interactable = true;
            EnemySlider.interactable = true;
        }
        else
        {
            PlayerSlider.interactable = false;
            DragonSlider.interactable = false;
            DragonManaSlider.interactable = false;
            EnemySlider.interactable = false;
        }
    }

    void GeneralBack()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            if (TargetSelectPanel.gameObject.activeSelf)
            {
                ShowTargetPanel(false);
                return;
            }

            if (DragonSpellsPanel.gameObject.activeSelf)
            {
                ShowDragonSpellsPanel(false);
                ShowDragonPanel(true);
                return;
            }

            if (DragonSelectPanel.gameObject.activeSelf)
            {
                ShowDragonSelectPanel(false);
                ShowDragonPanel(true);
                return;
            }

            if (DragonPanel.gameObject.activeSelf)
            {
                ShowDragonPanel(false);
                ShowMainMenu(true);
                return;
            }

            if (PlayerPanel.gameObject.activeSelf)
            {
                ShowPlayerPanel(false);
                ShowMainMenu(true);
                return;
            }
        }
    }

    void ShowMainMenu(bool active)
    {
        //MainPanel.SetActive(active);

        if (PlayerQueued)
        {
            PlayerButton.interactable = false;
        }
        else
        {
            PlayerButton.interactable = active;
        }

        if (DragonQueued || CM.Player.AllDragonsDead())
        {
            DragonButton.interactable = false;
        }
        else
        {
            DragonButton.interactable = active;
        }

        if (CM.Dragon == null)
            DragonButton.interactable = false;

        if (PlayerQueued || DragonQueued || GlobalFlags.GetBossFlag() == true)
        {
            CaptureButton.interactable = false;
        }
        else
        {
            CaptureButton.interactable = active;
        }

        if (GlobalFlags.GetBossFlag() == true)
        {
            FleeButton.interactable = false;
        }
        else
        {
            FleeButton.interactable = active;
        }

        if (DragonCaptured)
        {
            PlayerButton.interactable = false;
            DragonButton.interactable = false;
            CaptureButton.interactable = false;
            FleeButton.interactable = false;
        }

        //ES.SetSelectedGameObject(PlayerButton.gameObject);
    }

    void ShowPlayerPanel(bool active)
    {
        InfoTextPanel.SetActive(!active);
        
        PlayerPanel.SetActive(active);

        //ES.SetSelectedGameObject(PlayerAttack.gameObject);
    }

    void ShowDragonPanel(bool active)
    {
        InfoTextPanel.SetActive(!active);

        DragonPanel.SetActive(active);

        //ES.SetSelectedGameObject(DragonAttack.gameObject);
    }

    void ShowDragonSpellsPanel(bool active)
    {
        DragonSpellsPanel.SetActive(active);

        DragonAttack.interactable = !active;
        DragonMagic.interactable = !active;
        DragonDefend.interactable = !active;
        DragonSwitch.interactable = !active;
        DragonBack.interactable = !active;

        /*
        if(SpellButton4.interactable = CM.DragonSpellCastable(3))
            ES.SetSelectedGameObject(SpellButton4.gameObject);
        if(SpellButton3.interactable = CM.DragonSpellCastable(2))
            ES.SetSelectedGameObject(SpellButton3.gameObject);
        if(SpellButton2.interactable = CM.DragonSpellCastable(1))
            ES.SetSelectedGameObject(SpellButton2.gameObject);
        if(SpellButton1.interactable = CM.DragonSpellCastable(0))
            ES.SetSelectedGameObject(SpellButton1.gameObject);
        */
    }

    void ShowDragonSelectPanel(bool active)
    {
        DragonSelectPanel.SetActive(active);

        DragonAttack.interactable = !active;
        DragonMagic.interactable = !active;
        DragonDefend.interactable = !active;
        DragonSwitch.interactable = !active;
        DragonBack.interactable = !active;

        /*
        if (DragonSelectButton4.interactable = CM.IsDragonAlive(3))
            ES.SetSelectedGameObject(DragonSelectButton4.gameObject);
        if (DragonSelectButton3.interactable = CM.IsDragonAlive(2))
            ES.SetSelectedGameObject(DragonSelectButton4.gameObject);
        if (DragonSelectButton2.interactable = CM.IsDragonAlive(1))
            ES.SetSelectedGameObject(DragonSelectButton4.gameObject);
        if (DragonSelectButton1.interactable = CM.IsDragonAlive(0))
            ES.SetSelectedGameObject(DragonSelectButton4.gameObject);
        */

        if ((DragonNames[3].CompareTo("(Empty)") == 0 || !CM.IsDragonAlive(3)) && dst != DragonSelectType.ReleasingDragon)
        {
            DragonSelectButton4.interactable = false;
        }
        else
        {
            DragonSelectButton4.interactable = true;
        }

        if ((DragonNames[2].CompareTo("(Empty)") == 0 || !CM.IsDragonAlive(2)) && dst != DragonSelectType.ReleasingDragon)
        {
            DragonSelectButton3.interactable = false;
        }
        else
        {
            DragonSelectButton3.interactable = true;
        }

        if ((DragonNames[1].CompareTo("(Empty)") == 0 || !CM.IsDragonAlive(1)) && dst != DragonSelectType.ReleasingDragon)
        {
            DragonSelectButton2.interactable = false;
        }
        else
        {
            DragonSelectButton2.interactable = true;
        }

        if ((DragonNames[0].CompareTo("(Empty)") == 0 || !CM.IsDragonAlive(0)) && dst != DragonSelectType.ReleasingDragon)
        {
            DragonSelectButton1.interactable = false;
        }
        else
        {
            DragonSelectButton1.interactable = true;
        }
    }

    void ShowTargetPanel(bool active)
    {
        TargetSelectPanel.SetActive(active);

        SpellButton1.interactable = !active;
        SpellButton2.interactable = !active;
        SpellButton3.interactable = !active;
        SpellButton4.interactable = !active;

        //ES.SetSelectedGameObject(TargetSelectPlayer.gameObject);
        if (CM.Dragon == null)
            TargetSelectDragon.interactable = false;
    }

    void PlayerButtonClick()
    {
        ShowMainMenu(false);
        ShowPlayerPanel(true);
    }

    void DragonButtonClick()
    {
        ShowMainMenu(false);
        ShowDragonPanel(true);
    }

    void CaptureButtonClick()
    {
        CM.QueuePlayer(2);
    }

    void FleeButtonClick()
    {
        CM.QueuePlayer(4);

        // Set flee
        Player.SetEnterBattleDelayTimer();
    }

    void PlayerAttackClick()
    {
        ShowPlayerPanel(false);
        ShowMainMenu(true);
        CM.QueuePlayer(1);
    }

    /// <summary>
    /// Method which activates inventory canvas when inventory button is clicked
    /// </summary>
    void PlayerInventoryClick()
    {
        InventoryCanvasController.instance.Activate();
        InventoryCanvasController.instance.Combat = true;
    }

    /// <summary>
    /// Method that handles when use button is clicked in combat
    /// </summary>
    void PlayerInventoryUseClick()
    {
        CM.QueueItem();
        InventoryCanvasController.instance.ItemDialog.gameObject.SetActive(true);
        ItemTextBox = true;
    }

    /// <summary>
    /// Method that handles when cancel button is clicked in combat
    /// </summary>
    void PlayerInventoryCancelClick()
    {
        InventoryCanvasController.instance.UseButton.gameObject.SetActive(false);
        InventoryCanvasController.instance.CancelButton.gameObject.SetActive(false);
        InventoryCanvasController.instance.InventoryStartUp.ClearItemDescription();
    }

    void PlayerDefendClick()
    {
        ShowPlayerPanel(false);
        ShowMainMenu(true);
        CM.QueuePlayer(3);
    }

    void PlayerBackClick()
    {
        ShowPlayerPanel(false);
        ShowMainMenu(true);
    }

    void DragonAttackClick()
    {
        ShowDragonPanel(false);
        ShowMainMenu(true);
        CM.QueueDragon(1);
    }

    void DragonMagicClick()
    {
        //useSpell = true;
        ShowDragonSpellsPanel(true);
    }

    void DragonDefendClick()
    {
        ShowDragonPanel(false);
        ShowMainMenu(true);
        CM.QueueDragon(4);
    }

    void DragonSwitchClick()
    {
        dst = DragonSelectType.SwappingDragon;
        ShowDragonSelectPanel(true);
    }

    void DragonBackClick()
    {
        ShowDragonPanel(false);
        ShowMainMenu(true);
    }

    void SpellSelect(int code)
    {
        SpellCode = code;

        TargetSelectPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.25f * code, 1.1f);
        TargetSelectPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.25f + (0.25f * code), 2.6f);

        ShowTargetPanel(true);
    }

    void DragonSelect(int code, DragonSelectType dst)
    {
        DragonCode = code;

        ShowDragonSelectPanel(false);
        ShowDragonPanel(false);
        ShowMainMenu(true);

        //dragon select is being used to choose which dragon to swap in
        if (dst == DragonSelectType.SwappingDragon)
            CM.QueueDragonSwitch(DragonCode);
        //dragon select is being used to choose which dragon to release
        else if (dst == DragonSelectType.ReleasingDragon)
            CM.ReplaceDragon(DragonCode);
        //dragon select is being used to choose which dragon to send out after one has died
        else
            CM.SwitchDragon(DragonCode);
    }

    void TargetSelect(int code)
    {
        TargetCode = code;

        ShowTargetPanel(false);
        ShowDragonSpellsPanel(false);
        ShowDragonPanel(false);
        ShowMainMenu(true);

        CM.QueueDragonSpell(SpellCode, TargetCode);
    }

    void GetSliderInfo()
    {
        float[] playerInfo = new float[2];
        float[] dragonInfo = new float[4];
        float[] enemyInfo = new float[2];

        playerInfo = CM.GetPlayerInfo();
        dragonInfo = CM.GetDragonInfo();
        enemyInfo = CM.GetEnemyInfo();

        PlayerSlider.maxValue = playerInfo[0];
        PlayerSlider.value = playerInfo[1];

        DragonSlider.maxValue = dragonInfo[0];
        DragonSlider.value = dragonInfo[1];
        DragonManaSlider.maxValue = dragonInfo[2];
        DragonManaSlider.value = dragonInfo[3];

        EnemySlider.maxValue = enemyInfo[0];
        EnemySlider.value = enemyInfo[1];
    }

    public void UpdateSliders(float playerHealth, float dragonHealth, float dragonMana, float enemyHealth)
    {
        PlayerSlider.value = playerHealth;
        DragonSlider.value = dragonHealth;
        DragonManaSlider.value = dragonMana;
        EnemySlider.value = enemyHealth;
    }

    /// <summary>
    /// Method that updates only players health bar slider
    /// </summary>
    /// <param name="playerHealth"></param>
    public void UpdatePlayerSlider(float playerHealth)
    {
        PlayerSlider.value = playerHealth;
    }

    public void DisplayLine(string s)
    {
        string buffer = "";

        for (int i = 0; i < TextQueueLength - 1; i++)
        {
            TextQueue[i] = TextQueue[i + 1];
            buffer = buffer + TextQueue[i] + '\n';
        }

        TextQueue[TextQueueLength - 1] = s;
        buffer = buffer + TextQueue[TextQueueLength - 1];

        InfoTextPanel.GetComponentInChildren<Text>().text = buffer;
    }

    public void Wait()
    {
        ShowMainMenu(false);
        //ES.SetSelectedGameObject(PlayerButton.gameObject);
        IsHalted = true;
    }

    /*public void ReplaceSpell()
    {
        useSpell = false;
        DisplayLine(CM.Dragon.GetName()"");
    }*/

    //choose a dragon to replace one that has run out of health
    public void SendOutDragon()
    {
        dst = DragonSelectType.SendingOutDragon;
        ShowDragonSelectPanel(true);
    }

    //replace an old dragon with a newly captured one
    public void ReplaceDragon()
    {
        dst = DragonSelectType.ReleasingDragon;
        DisplayLine("Party is full.");
        DisplayLine("Choose a dragon to release.");
        ShowDragonSelectPanel(true);
    }

    void ColorInit()
    {
        // Set up color blocks
        ColorBlock PlayerColors = ColorBlock.defaultColorBlock;
        PlayerColors.normalColor = new Color32(BaseColor, 0x00, 0x00, 0xFF);
        PlayerColors.highlightedColor = new Color32(HighlightedColor, 0x00, 0x00, 0xFF);
        PlayerColors.pressedColor = new Color32(ClickedColor, 0x00, 0x00, 0xFF);

        ColorBlock DragonColors = ColorBlock.defaultColorBlock;
        DragonColors.normalColor = new Color32(0x00, 0x00, BaseColor, 0xFF);
        DragonColors.highlightedColor = new Color32(0x00, 0x00, HighlightedColor, 0xFF);
        DragonColors.pressedColor = new Color32(0x00, 0x00, ClickedColor, 0xFF);

        ColorBlock CaptureColors = ColorBlock.defaultColorBlock;
        CaptureColors.normalColor = new Color32(0x00, BaseColor, 0x00, 0xFF);
        CaptureColors.highlightedColor = new Color32(0x00, HighlightedColor, 0x00, 0xFF);
        CaptureColors.pressedColor = new Color32(0x00, ClickedColor, 0x00, 0xFF);

        ColorBlock FleeColors = ColorBlock.defaultColorBlock;
        FleeColors.normalColor = new Color32(BaseColor, BaseColor, 0x00, 0xFF);
        FleeColors.highlightedColor = new Color32(HighlightedColor, HighlightedColor, 0x00, 0xFF);
        FleeColors.pressedColor = new Color32(ClickedColor, ClickedColor, 0x00, 0xFF);

        ColorBlock EnemyColors = ColorBlock.defaultColorBlock;
        EnemyColors.normalColor = new Color32(BaseColor, 0x00, BaseColor, 0xFF);
        EnemyColors.highlightedColor = new Color32(HighlightedColor, 0x00, HighlightedColor, 0xFF);
        EnemyColors.pressedColor = new Color32(ClickedColor, 0x00, ClickedColor, 0xFF);

        // Main panel
        PlayerButton.colors = PlayerColors;
        DragonButton.colors = DragonColors;
        CaptureButton.colors = CaptureColors;
        FleeButton.colors = FleeColors;

        // Player panel
        PlayerAttack.colors = PlayerColors;
        PlayerInventory.colors  = PlayerColors;
        PlayerDefend.colors = PlayerColors;
        PlayerBack.colors = PlayerColors;

        // Dragon panel
        DragonAttack.colors = DragonColors;
        DragonMagic.colors = DragonColors;
        DragonDefend.colors = DragonColors;
        DragonSwitch.colors = DragonColors;
        DragonBack.colors = DragonColors;

        // Dragon select
        DragonSelectButton1.colors = DragonColors;
        DragonSelectButton2.colors = DragonColors;
        DragonSelectButton3.colors = DragonColors;
        DragonSelectButton4.colors = DragonColors;

        // Spells
        SpellButton1.colors = DragonColors;
        SpellButton2.colors = DragonColors;
        SpellButton3.colors = DragonColors;
        SpellButton4.colors = DragonColors;

        // Target select
        TargetSelectPlayer.colors = PlayerColors;
        TargetSelectDragon.colors = DragonColors;
        TargetSelectEnemy.colors = EnemyColors;
    }

    public void QueuePlayer()
    {
        PlayerQueued = true;
        ShowMainMenu(true);
        //PlayerButton.interactable = false;
        //CaptureButton.interactable = false;
    }

    public void QueueDragon()
    {
        DragonQueued = true;
        ShowMainMenu(true);
        //DragonButton.interactable = false;
        //CaptureButton.interactable = false;
    }

    public void ResetQueue()
    {
        PlayerQueued = false;
        DragonQueued = false;

        ShowMainMenu(true);
        //PlayerButton.interactable = true;
        //DragonButton.interactable = true;
        //CaptureButton.interactable = true;
    }

    public void CaptureDragon()
    {
        DragonCaptured = true;
        ShowMainMenu(false);
    }

    void PlayerCheat(float x = 0.0f)
    {
        CM.PlayerCheat(PlayerSlider.value);
    }

    void DragonCheat(float x = 0.0f)
    {
        CM.DragonCheat(DragonSlider.value, DragonManaSlider.value);
    }

    void EnemyCheat(float x = 0.0f)
    {
        CM.EnemyCheat(EnemySlider.value);
    }
}
