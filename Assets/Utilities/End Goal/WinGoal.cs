using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGoal : MonoBehaviour {
    bool hasWon = false;

    public GameObject WinUI;
    public GameObject LoseUI;
    public GameObject Hud;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player" && !hasWon) {
            FirstPersonPlayer player = other.GetComponent<FirstPersonPlayer>();
            hasWon = true;

            Debug.Log("HasWon - Trigger");

            player.enabled = false;

            if(Hud != null)
                Hud.SetActive(false);

            if(WinUI != null)
                WinUI.SetActive(true);

            SceneManager.LoadScene(0);
        }
    }
}
