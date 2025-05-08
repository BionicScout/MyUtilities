using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialougeBox : MonoBehaviour {
    public TMP_Text dialogueText;
    public TMP_Text personText;
    public TMP_Text nextButtonText;
    public Button nextButton;
    public List<Button> choiceButtons;

    public Action<int> Action_ChoiceButton;
    public Action Action_NextButton; 

    private void Awake() {
        foreach (var button in choiceButtons) {
            button.gameObject.SetActive(false);
        }
        nextButton.gameObject.SetActive(true);

        OnClick_NextButton();
    }

    public void SetDialogue(string dialogue) {
        dialogueText.text = dialogue;

        foreach(var button in choiceButtons) {
            button.gameObject.SetActive(false);
        }
        nextButton.gameObject.SetActive(true);
    }

    public void SetChoice(int choice, string text) {
        nextButton.gameObject.SetActive(false);

        choiceButtons[choice].transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        choiceButtons[choice].gameObject.SetActive(true);
    }

    public void SetPerson(string text) {
        personText.text = text;
    }

    public void SetNextButton(string text) {
        nextButtonText.text = "-> " + text;
    }

    public void OnClick_NextButton() {
        Action_NextButton?.Invoke();
    }
    public void OnClick_ChoiceButton(int index) {
        Action_ChoiceButton?.Invoke(index);
    }
}
