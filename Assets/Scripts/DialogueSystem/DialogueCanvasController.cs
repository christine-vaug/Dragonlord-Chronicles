using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCanvasController : MonoBehaviour {

    public static DialogueCanvasController instance;


    public GameObject choicesCanvas;
    public GameObject dialogueCanvas;

    public Text dialogueText;
    public GameObject choicesWindow;
    public GameObject choiceButtonPrefab;

    private System.Action OnBranchTaken;

    enum CanvasMode {
        NPCText,
        PlayerText
    }


    static DialogueTree tree;
    static CanvasMode mode;

	// Use this for initialization
	void Awake () {

        instance = this;
        Deactivate();
    }

    void Start () {

    }

    // Update is called once per frame
    void Update () {

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && mode == CanvasMode.NPCText) {

            if (tree.FinishedAdvancingSentences() == false) {

                dialogueText.text = tree.AdvanceToNextSentence();

            } else {

                SwapModes();
            }
        }
        
	}
    
    void SwapModes () {

        if (mode == CanvasMode.NPCText) {
            choicesCanvas.SetActive(true);
            dialogueCanvas.SetActive(false);

            DialogueChoice[] choices = tree.GetChoices();

            if (choices != null && choices.Length > 0) {

                for (int i = 0; i < choices.Length; i++) {

                    GameObject buttonGO = Instantiate(choiceButtonPrefab);
                    buttonGO.transform.SetParent(choicesWindow.transform);

                    Button button = buttonGO.GetComponent<Button>();
                    button.GetComponentInChildren<Text>().text = choices[i].successSentence;
                    button.GetComponentInChildren<Text>().color = new Color(255, 255, 255, 255);

                    button.onClick.AddListener(() => OnChoiceClicked(button));

                    button.name = i.ToString() + " " + choices[i].successSentence;
                }

            } else {

                //Is there a branch to take?
                if (OnBranchTaken != null) OnBranchTaken();

                //Reached the end of tree
                Deactivate();
            }



            mode = CanvasMode.PlayerText;
            return;
        } else {

            choicesCanvas.SetActive(false);
            dialogueCanvas.SetActive(true);

            dialogueText.text = tree.AdvanceToNextSentence();

            int children = choicesWindow.transform.childCount;

            for (int i = 0; i < children; i++) {

                Destroy(choicesWindow.transform.GetChild(i).gameObject);

            }

            mode = CanvasMode.NPCText;
            return;
        }
    }

    void OnChoiceClicked (Button button) {

        string[] split = button.name.Split(' ');
        int choice = int.Parse(split[0]);
        
        OnBranchTaken = null;
        if (split.Length > 2 && split[2].Contains("(SHOP)")) {
            OnBranchTaken = OnShop;
        }
        if (split.Length > 2 && split[2].Contains("(SLEEP)")) {
            OnBranchTaken = OnUseInn;
        }
        if (split.Length > 2 && split[2].Contains("(BATTLE)")) {
            OnBranchTaken = OnBattle;
        }

        tree.AdvancePath(choice);
        SwapModes();
    }

    public void SetDialogueTree (string name) {

        tree = DialogueTree.LoadFromFile(name);
    }

    public void Activate () {

        gameObject.SetActive(true);

        choicesCanvas.SetActive(false);
        dialogueCanvas.SetActive(true);

        mode = CanvasMode.NPCText;
        dialogueText.text = tree.AdvanceToNextSentence();
    }

    public void Deactivate () {

        gameObject.SetActive(false);
    }

    public bool IsFinished () {

        return !gameObject.activeInHierarchy;
    }
    

    public void OnShop () {
        
        ShopkeeperCanvasController.Instance.Activate(GlobalFlags.ShopKeeperData, GlobalFlags.PlayerInventory, GlobalFlags.PlayerData);

    }

    public void OnUseInn () {

        float gold = GlobalFlags.PlayerData.Gold;

        if (gold >= 10) {

            GlobalFlags.PlayerData.CurrentHealth = GlobalFlags.PlayerData.MaxHealth;

            GlobalFlags.PlayerData.AddGold(-10.0f);
            GameManager.instance.PopState();

        }
        
    }



    public void OnBattle () {

        GameManager.instance.PushState(StateManagement.GameStateType.Battle);
    }
}
