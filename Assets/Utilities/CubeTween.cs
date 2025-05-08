using Ink.Parsed;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class CubeTween : MonoBehaviour
{
    public float lerpSpeed;
    private float lerp = 0;
    private Vector3 startPos;
    private Vector3 endPos;

    private void Start() {
        startPos = transform.position;
        endPos = transform.position + Vector3.right * 10;
    }

    private void Update() {
        lerp += lerpSpeed * Time.deltaTime;
        lerp = Mathf.Clamp01(lerp);
        transform.position = Vector3.Lerp(startPos, endPos, EaseInOutCubic(lerp));
    }

    public float EaseInOutCubic(float x) {
        return x < 0.5 ? 4 * x* x* x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}
