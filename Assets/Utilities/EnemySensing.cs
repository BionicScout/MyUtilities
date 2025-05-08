using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// 
/// THIS SCRIPT NEEDS REWORKED TO BE MORE GENERAL
/// 

public class EnemySensing : MonoBehaviour {
    GameObject player;

    private enum ObserveState{ nothing = 0, hearing = 1, sight = 2, hearingAndSight = 3 }
    private ObserveState playerObserveState = ObserveState.nothing;

    private enum SeeEnemyAlertedState{ nothing = 0, playerSeen = 1, playerLost = 2};
    private SeeEnemyAlertedState currentSeeEnemyState = SeeEnemyAlertedState.nothing;

    private enum DetectionState { notAware = 0, suspicious = 1, alert = 2 }
    private DetectionState playerDetectionState = DetectionState.notAware;

    [Header("Senses")]
    public float seeingRange = 30f;
    public float timeToAlert = 6f;
    public float hearingImpact = 0.2f;

    Vector3 lastKnownSpot = Vector3.zero;
  

    [Header("DetectionBar")]
    public Slider detectionMeter;
    public float dectectionReductionTime = 10f;

    public Color notDetectedColor;
    public Color suspiciousColor;
    public Color alertColor;

    float detectionValue = 0;
    bool pauseDecreasce = false;
    public bool alertedOnce;




    /************************************************************************************
        Variable Setters
    ************************************************************************************/
    private void adjustDetection(float offset) {
        detectionValue += offset;
        detectionValue = Mathf.Clamp(detectionValue, 0, 1);
    }

    /************************************************************************************
        Unity Methods
    ************************************************************************************/
    public void Start() {
        FirstPersonPlayer playerScript = FindAnyObjectByType<FirstPersonPlayer>();
        if(playerScript == null) {
            Debug.LogError("EnemySensing: No player found");
        }
        player = playerScript.gameObject;
    }


    private void Update() {
        playerObserveState = ObserveState.nothing;
        SpotPlayer();

        currentSeeEnemyState = SeeEnemyAlertedState.nothing;
        SpotAlertEnemy();

        if(playerObserveState == ObserveState.nothing && !pauseDecreasce) {
            float detectionOffset = -(Time.deltaTime / dectectionReductionTime);
            adjustDetection(detectionOffset);
        }

        UpdateDetectionBar();
    }

    private void LateUpdate() {
        detectionMeter.gameObject.transform.parent.LookAt(player.transform);


        //Debug.Log("Detection Value: " + detectionValue);
        if(playerDetectionState != DetectionState.alert && detectionValue == 1) {
            playerDetectionState = DetectionState.alert;
        }
        else if(playerDetectionState != DetectionState.suspicious && detectionValue < 1 && detectionValue >= 0.5f) {
            playerDetectionState = DetectionState.suspicious;
        }
        else if (playerDetectionState != DetectionState.notAware && detectionValue < 0.5f) {
            playerDetectionState = DetectionState.notAware;
        }

        //Debug.Log("State: " + playerObserveState.ToString());
    }


    /************************************************************************************
        Senses
    ************************************************************************************/

    private void SpotPlayer() {
        bool playerSpotted = SpotTarget(player.transform.position, "Player");

        if(!playerSpotted) { return; }

        //Seen
        if(alertedOnce) { 
            SetDectionToMax();
        }
        else {
            float detectionOffset = Time.deltaTime / timeToAlert;
            adjustDetection(detectionOffset);
        }

        lastKnownSpot = player.transform.position;

        if(playerObserveState == ObserveState.nothing)
            playerObserveState = ObserveState.sight;
        else if(playerObserveState == ObserveState.hearing)
            playerObserveState = ObserveState.hearingAndSight;
    }

    private void SpotAlertEnemy() {
        //EnemyAI[] enemies = Resources.FindObjectsOfTypeAll<EnemyAI>();

        //foreach(EnemyAI selectedEnemy in enemies) {
        //    //Correct State
        //    if(selectedEnemy.currentState != EnemyAI.stateId.alert_PlayerDetected && selectedEnemy.currentState != EnemyAI.stateId.alert_LostPlayer) {
        //        continue;
        //    }

        //    //Detection
        //    bool enemySpotted = SpotTarget(player.transform.position , "Enemy");
        //    if(!enemySpotted) { continue; }

        //    //Seen
        //    SetDectionToMax();

        //    if(selectedEnemy.currentState != EnemyAI.stateId.alert_PlayerDetected) {
        //        lastKnownSpot = selectedEnemy.GetPlayerLastLoc();
        //        currentSeeEnemyState = SeeEnemyAlertedState.playerSeen;
        //        break;
        //    }
        //    if(selectedEnemy.currentState != EnemyAI.stateId.alert_LostPlayer) {
        //        currentSeeEnemyState = SeeEnemyAlertedState.playerLost;
        //        continue;
        //    }
        //}
    }

    private void UpdateDetectionBar() {
        Image sliderColorBar = detectionMeter.transform.GetChild(1).GetChild(0).GetComponent<Image>();

        if(detectionValue < 0.5f){
            sliderColorBar.color = notDetectedColor;
        }
        else if(detectionValue < 1) {
            sliderColorBar.color = suspiciousColor;
        }
        else if (detectionValue == 1) {
            sliderColorBar.color = alertColor;
        }

        detectionMeter.value = detectionValue / 1f;
    }


    public void CheckIfHeard(SoundClass sound) {
        if(alertedOnce) {
            SetDectionToMax();
        }
        else {
            adjustDetection(hearingImpact);
        }
        lastKnownSpot = sound.position;

        if(playerObserveState == ObserveState.nothing)
            playerObserveState = ObserveState.hearing;
        else if(playerObserveState == ObserveState.sight)
            playerObserveState = ObserveState.hearingAndSight;
    }

    /************************************************************************************
        Sight Subfunctions
    ************************************************************************************/
    private bool SpotTarget(Vector3 targetPos, string targetTag) {
        Vector3 enemyPos = transform.position;

        Vector3 enemyForward = transform.forward;
        Vector3 targetDirection = (targetPos.x - enemyPos.x) * Vector3.right + (targetPos.z - enemyPos.z) * Vector3.forward;
        targetDirection = Vector3.Normalize(targetDirection);

        float angle = Vector3.Dot(enemyForward , targetDirection);

        if(angle < 0) {
            return false; //Is not Spotted
        }

        float distanceToTarget = Vector3.Distance(targetPos , enemyPos);
        if(distanceToTarget > seeingRange) {
            return false; //Is not Spotted
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position , targetDirection , out hit , distanceToTarget)) {
            if(hit.collider.gameObject.tag != targetTag) {
                return false; //Is not Spotted
            }
        }

        return true; //Is Spotted
    }


    /************************************************************************************
        Simple Functions for other classes
    ************************************************************************************/
    public Vector3 PlayerLastLoc() {
        return lastKnownSpot;
    }

    public bool IsUnaware() {
        return playerDetectionState == DetectionState.notAware;
    }

    public bool IsSuspicious() {
        return playerDetectionState == DetectionState.suspicious;
    }

    public bool IsAlert() {
        return playerDetectionState == DetectionState.alert;
    }

    public void PauseDecrease() {
        pauseDecreasce = true;
    }
    public void UnpauseDecrease() {
        pauseDecreasce = false;
    }

    public void SetDectionToMax() {
        detectionValue = 1;
    }

    public bool IsPlayerDetected() {
        return playerObserveState != ObserveState.nothing;
    }

    public bool CanSeeAlerted_PlayerKnown() {
        return currentSeeEnemyState == SeeEnemyAlertedState.playerSeen;
    }

    public bool CanSeeAlerted_PlayerLost() {
        return currentSeeEnemyState == SeeEnemyAlertedState.playerLost;
    }
}
