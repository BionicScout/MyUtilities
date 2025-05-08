using UnityEngine;

public class FirstPersonPlayer : MonoBehaviour {    
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float runSpeed = 20f;
    public float crouchSpeed = 5f;
    public float crouchHeight = 0.8f;
    public float heightChangeSpeed = 0.01f;

    private float normalHeight = 1f;
    private float lerpHeight = 0f;


    CharacterController characterController;

    public enum MoveState { walk = 0, run = 1, crouch = 2 };
    MoveState currentMovestate = MoveState.walk;

    [Header("Gravity")]
    public float gravity = 9.81f;
    public float maxGravitySpeed = 25f;
    public Collider floorCollider;

    float currentGravitySpeed;

    [Header("Camera")]
    public float vertSensitivity = 300f;
    public float horzSensitivity = 300f;
    public float maxPitch = 90f;
    public float minPitch = -90f;

    Camera cam;
    float pitch = 0f;

    [Header("Sounds")]
    public SO_SoundClip footstepSound;
    public AudioSource footstepSource;

    private void Start() {
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        UpdateMoveState();
        RotateCamera();
        MovePlayer();
        UpdateHeight();
        Gravity();
    }

    private void UpdateHeight() {
        float heightDifference = normalHeight - crouchHeight;
        float lerpOffset = heightDifference * Time.deltaTime / heightChangeSpeed;

        if(currentMovestate == MoveState.crouch) {
            lerpOffset *= -1;
        }

        lerpHeight += lerpOffset;
        lerpHeight = Mathf.Clamp(lerpHeight, crouchHeight, normalHeight);

        Vector3 playerScale = transform.localScale;
        transform.localScale = playerScale.x * Vector3.right + lerpHeight * Vector3.up + playerScale.z * Vector3.forward;
    }

    public void UpdateMoveState() {
        bool running = currentMovestate == MoveState.run;
        bool movingForward = Input.GetAxis("Vertical") > 0;
        bool runButtonPressed = Input.GetKeyDown(KeyCode.LeftControl);

        if(runButtonPressed && !running && movingForward) {
            currentMovestate = MoveState.run;
        }
        else if(running && !movingForward) {
            currentMovestate = MoveState.walk;
        }

        bool crouching = currentMovestate == MoveState.crouch;
        bool crouchButtonPressed = Input.GetKeyDown(KeyCode.C);

        if(crouchButtonPressed && !crouching) {
            currentMovestate = MoveState.crouch;
        }
        else if (crouchButtonPressed && crouching) {
            currentMovestate = MoveState.walk;
        }
    }

    private void MovePlayer() {
        float forwardAxis = Input.GetAxis("Vertical");
        float sideAxis = Input.GetAxis("Horizontal");

        Vector3 rawMoveOffset = transform.forward * forwardAxis + transform.right * sideAxis;
        Vector3 finalMoveOffset = Vector3.Normalize(rawMoveOffset) * moveSpeed * Time.deltaTime;
        if(currentMovestate == MoveState.run) { finalMoveOffset *= runSpeed / moveSpeed; }
        else if(currentMovestate == MoveState.crouch) { finalMoveOffset *= crouchSpeed / moveSpeed; }

        characterController.Move(finalMoveOffset);

        if(finalMoveOffset.magnitude > 0 && !footstepSource.isPlaying && currentMovestate != MoveState.crouch) {
            if(currentMovestate == MoveState.run) {
                SoundClass sound = new SoundClass(footstepSource , footstepSound.clip , footstepSound.range * 2, this.transform.position);
                sound.Play();
            }
            else {
                SoundClass sound = new SoundClass(footstepSource , footstepSound.clip , footstepSound.range , this.transform.position);
                sound.Play();
            }
        }
    }

    public void Gravity() {
        Vector3 gravityMove = Vector3.down * currentGravitySpeed * Time.deltaTime;
        characterController.Move(gravityMove);

        if(characterController.isGrounded) {
            currentGravitySpeed = 0.5f;
        }
        else {
            currentGravitySpeed += gravity * Time.deltaTime;
            currentGravitySpeed = Mathf.Min(currentGravitySpeed , maxGravitySpeed);
        }
    }

    private void RotateCamera() {
        float yaw = Input.GetAxis("Mouse X") * horzSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * vertSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cam.transform.localRotation = Quaternion.Euler(pitch , 0f , 0f);
        transform.Rotate(Vector3.up * yaw);
    }

}
