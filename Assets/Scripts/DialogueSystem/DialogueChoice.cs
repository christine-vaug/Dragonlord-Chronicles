using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChoice {

    public DialogueRequirement requirement;

    public string successSentence;
    public DialogueTreeNode successNext;

    public string failureSentence;
    public DialogueTreeNode failureNext;
}
