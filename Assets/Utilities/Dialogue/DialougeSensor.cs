using Unity.VisualScripting;
using UnityEngine;

public class DialougeSensor : MonoBehaviour {
    bool playerIn = false;

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player") {
            playerIn = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            playerIn = true;
        }
    }

    private void Update() {
        if(playerIn && Input.GetKeyDown(KeyCode.E)) {
            FindAnyObjectByType<DialogueManager>().StartDialouge();
        }
    }
}
