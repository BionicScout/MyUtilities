using UnityEngine;

[CreateAssetMenu(fileName = "SO_SoundClipo" , menuName = "Scriptable Objects/SO_SoundClipo")]
public class SO_SoundClip : ScriptableObject {
    public AudioClip clip;
    public float range;
}
