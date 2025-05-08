using UnityEngine;

public class SoundClass {
    public AudioSource source;
    public AudioClip clip;
    public float range;
    public Vector3 position;

    public SoundClass(AudioSource source , AudioClip clip , float range , Vector3 position) {
        this.source = source;
        this.clip = clip;
        this.range = range;
        this.position = position;
    }

    public void Play() {
        source.clip = clip;
        source.transform.position = position;
        source.Play();

        RangeNotify();
    }

    /// <summary>
    /// Gets all the EnemyHearingScript instances in range and notifies them that this sound has been played.
    /// </summary>
    protected virtual void RangeNotify() {
        Collider[] listeners = Physics.OverlapSphere(position, range);

        //For each collider in range
        foreach(var listener in listeners) {
            //If the game object with the collider has an EnemyHearingScript, 
            if(listener.TryGetComponent<EnemySensing>(out var enemySensing)) {
                //Tell it to check if they could hear this sound
                enemySensing.CheckIfHeard(this);
            }
        }
    }
}
