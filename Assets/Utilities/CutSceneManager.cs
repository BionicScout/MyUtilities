using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour {
    public List<AudioClip> clips;
    public AudioSource audioSource;
    public Transform audioPosition;
    public float clipDelay = 0.5f;
    public PlayableDirector director;

    Camera previousCam;
    public Camera cutSceneCam;

    bool finished = false;

    [System.Serializable]
    public class AudioClip {
        public string id;
        public List<string> lang;
        public List<SO_SoundClip> sound;
    }

    public void StartDirector() {
        previousCam = Camera.main;
        previousCam.enabled = false;
        cutSceneCam.enabled = true;
        FindObjectOfType<FirstPersonPlayer>().enabled = false;
        director.Play();
    }

    public void Finished() { finished = true; }

    Queue<string> soundQueue = new Queue<string>();
    float timer = 0;

    private void Update() {
        if(soundQueue.Count > 0 && timer == 0) {
            PlayDialouge(soundQueue.Dequeue());
        }

        if(timer == 0 && finished) {
            Debug.Log("Exit");
            //director.Stop();

            previousCam.enabled = true;
            cutSceneCam.enabled = false;
            FindObjectOfType<FirstPersonPlayer>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(GetComponent<BoxCollider>());

            finished = false;
        }

        timer = Mathf.Max(0, timer - Time.deltaTime);
    }

    void PlayDialouge(string dialougeId) {
        AudioClip clip = clips.Find(x => x.id == dialougeId);
        if (clip == null) {
            return;
        }

        int langInd = clip.lang.IndexOf(CurrentLanguage.lang);
        SoundClass sound = new SoundClass(audioSource, clip.sound[langInd].clip, clip.sound[langInd].range, audioPosition.position);
        sound.Play();
        //Debug.Log("PLayed - " + dialougeId);
        timer = sound.clip.length + clipDelay;
    }

    public void QueueSound(string id) {
        soundQueue.Enqueue(id);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            StartDirector();
        }
    }
}
