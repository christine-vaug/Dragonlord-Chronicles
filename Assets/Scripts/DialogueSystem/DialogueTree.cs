using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTree {

    public DialogueTreeNode root;
    public DialogueTreeNode Current {
        get 
        {
            if (_current == null)  _current = root;
            return _current;
        }
        set 
        {
            currentSentence = 0;
            _current = value;
        }
    }

    public int currentSentence;

    private DialogueTreeNode _current;

    public bool FinishedAdvancingSentences () {

        return currentSentence == Current.sentences.Length;
    }

    public string AdvanceToNextSentence () {

        string result = Current.sentences[currentSentence];
        currentSentence++;
        return result;
    }

    public string[] GetSentences () {

        return Current.sentences;
    }

    public DialogueChoice[] GetChoices () {

        return Current.choices;
    }

    public DialogueTreeNode AdvancePath (int choiceIndex, bool success = true) {

        if (choiceIndex < 0 || choiceIndex >= Current.choices.Length) {
            Debug.LogError("Didn't work");
            return null;
        }

        if (success || Current.choices[choiceIndex].failureNext == null) {
            Current = Current.choices[choiceIndex].successNext;
            currentSentence = 0;
        } else {
            Current = Current.choices[choiceIndex].failureNext;
            currentSentence = 0;
        }

        return Current;
    }

    public void WriteTreeToDebugConsole () {

        Internal_WriteToDebugConsole(root);
    }

    private void Internal_WriteToDebugConsole (DialogueTreeNode node) {

        string sentences = "";
        for (int i = 0; i < node.sentences.Length; i++) {
            sentences += node.sentences[i];
        }
        Debug.Log(sentences);

        if (node.choices != null && node.choices.Length > 0) {

            for (int i = 0; i < node.choices.Length; i++) {

                Debug.Log(node.choices[i].successSentence);
                Internal_WriteToDebugConsole(node.choices[i].successNext);

                if (node.choices[i].failureNext != null) {

                    Debug.Log(node.choices[i].failureSentence);
                    Internal_WriteToDebugConsole(node.choices[i].failureNext);

                }
            }
        }
    }
   


    public static DialogueTree LoadFromFile (string name) {

        DialogueTree tree = new DialogueTree {
            root = CreateNodeFromFile(Application.streamingAssetsPath + "//DialogueTrees//" + name + "//", "_root.txt")
        };

        return tree;
    }
    
    static DialogueTreeNode CreateNodeFromFile (string path, string fileName) {

        DialogueTreeNode node = new DialogueTreeNode();
        string[] lines = System.IO.File.ReadAllLines(path + fileName);

        List<string> sentences = new List<string>();

        int index = 0;
        for ( ; index < lines.Length; index++) {
            
            if (lines[index].Contains("Choice:")) break;
            if (lines[index].Length > 0) sentences.Add(lines[index]);
        }

        List<DialogueChoice> choices = new List<DialogueChoice>();

        for (; index < lines.Length; index++) {

            if (lines[index].Contains("Choice:")) {

                DialogueChoice choice = new DialogueChoice();

                //Is this a success/failure choice?
                if (lines[index].Contains("[") && lines[index].Contains("]")) {

                    choice.requirement = new DialogueRequirement();

                    string requirementAsString = lines[index].Split('[')[1].Split(']')[0];
                    string[] split = requirementAsString.Split(' ');

                    choice.requirement.type = split[0];
                    choice.requirement.points = int.Parse(split[1]);

                    choice.successSentence = lines[index];
                    choice.failureSentence = lines[index + 1];
                    choice.successNext = CreateNodeFromFile(path, lines[index + 2]);
                    choice.failureNext = CreateNodeFromFile(path, lines[index + 3]);
                    index += 4;

                } else {

                    choice.successSentence = lines[index];
                    index++;
                    choice.successNext = CreateNodeFromFile(path, lines[index]);
                }


                choices.Add(choice);
            }
        }

        node.sentences = sentences.ToArray();
        node.choices = choices.ToArray();

        return node;
    }
    
}
