using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class DialogueManager : MonoBehaviour {
    [SerializeField] TextAsset scene;
    [SerializeField] DialougeBox dialougeBox;
    [SerializeField] LanguageManager languageManager;

    public GameObject door;

    private Story activeScene;
    bool inDialouge;

    public void Start() {
        dialougeBox.Action_NextButton += Next;
        dialougeBox.Action_ChoiceButton += MakeChoice;
    }

    public void Next() {
        if(!activeScene.canContinue) {
            EndDialouge();
            return;
        }

        //Set Dialogue
        string text = activeScene.Continue();
        dialougeBox.SetDialogue(text);

        if(activeScene.currentTags.Count > 0) {
            text = UpdateTextFromTag(activeScene.currentTags);  
            if(activeScene.currentTags.Count == 2) { dialougeBox.SetPerson(languageManager.GetLine(activeScene.currentTags[1]));  }
        }
        dialougeBox.SetDialogue(text);

        //If we have reached a point with a decision
        if(activeScene.currentChoices.Count > 0) {
            //For each choice
            for(int choiceIndex = 0; choiceIndex < activeScene.currentChoices.Count; choiceIndex++) {
                Choice choice = activeScene.currentChoices[choiceIndex];
                string choiceText = "-> " + choice.text;
                if(choice.tags != null && choice.tags.Count > 0) {
                    choiceText = "-> " + UpdateTextFromTag(choice.tags);
                }
                dialougeBox.SetChoice(choiceIndex, choiceText);
            }
        }               
    }

    public void MakeChoice(int index) {
        activeScene.ChooseChoiceIndex(index);
        Next();
    }

    private string UpdateTextFromTag(List<string> tags) {
        //Debug.Log(tags[0]);
        return languageManager.GetLine(tags[0]);
    }

    public void StartDialouge() {
        if(inDialouge)
            return;

        activeScene = new Story(scene.text);
        activeScene.BindExternalFunction("DestoryDoor", () =>  Destroy(door));
        dialougeBox.SetNextButton(languageManager.GetLine("NextButton"));
        dialougeBox.gameObject.SetActive(true);

        FindObjectOfType<FirstPersonPlayer>().enabled = false;
        Cursor.lockState = CursorLockMode.None;

        inDialouge = true;
    }

    public void EndDialouge() {
        dialougeBox.gameObject.SetActive(false);

        FindObjectOfType<FirstPersonPlayer>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        inDialouge = false;
    }

}