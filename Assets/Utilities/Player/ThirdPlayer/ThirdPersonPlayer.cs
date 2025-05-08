using UnityEngine;

public class ThirdPersonPlayer : MonoBehaviour {    
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float runSpeed = 20f;
    public float crouchSpeed = 5f;
    public float crouchHeight = 0.8f;
    public float heightChangeSpeed = 0.01f;

    private float normalHeight = 1f;
    private float lerpHeight = 0f;

    CharacterController characterController;

    public enum MoveState { idle = 0, walk = 1, run = 2, crouch = 3 };
    MoveState currentMovestate = MoveState.idle;

    [Header("Rotation")]
    public float rotationSpeed = 200f;
    public GameObject movementDirObject;

    [Header("Gravity")]
    public float gravity = 9.81f;
    public float maxGravitySpeed = 25f;
    public Collider floorCollider;

    float currentGravitySpeed;

    [Header("Camera")]
    public float vertSensitivity = 350f;
    public float horzSensitivity = 350f;
    public float maxPitch = 90f;
    public float minPitch = -90f;
    public GameObject cameraRotateObject;

    float pitch = 0f;
    float yaw = 0f;

    [Header("Animations")]
    public Animator animator;

    [Header("Sounds")]
    public SO_SoundClip footstepSound;
    public AudioSource footstepSource;

    private void Start() {
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
        RotatePlayerInMoveDir();
        UpdateHeight();
        Gravity();


        //Debug.Log(((int)currentMovestate));
        animator.SetInteger("MoveState", (int)currentMovestate);
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
        bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        
        if(!isMoving) {
            currentMovestate = MoveState.idle;
            //Debug.Log("idle");
        }
        else {
            currentMovestate = MoveState.walk;
        }

        bool running = currentMovestate == MoveState.run;
        bool movingForward = Input.GetAxis("Vertical") >= 0;
        bool runButtonPressed = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftShift);

        if(runButtonPressed && !running && movingForward) {
            currentMovestate = MoveState.run;
            //Debug.Log("run");
        }
        else if(running && !movingForward) {
            currentMovestate = MoveState.walk;
            //Debug.Log("run > walk");
        }

        bool crouching = currentMovestate == MoveState.crouch;
        bool crouchButtonPressed = Input.GetKeyDown(KeyCode.C);

        if(crouchButtonPressed && !crouching) {
            currentMovestate = MoveState.crouch;
            //Debug.Log("Crouch");
        }
        else if (crouchButtonPressed && crouching) {
            currentMovestate = MoveState.walk;
            //Debug.Log("crouch > walk");
        }
    }

    private void MovePlayer() {
        float forwardAxis = Input.GetAxis("Vertical");
        float sideAxis = Input.GetAxis("Horizontal");
        animator.SetFloat("Forward", forwardAxis);
        animator.SetFloat("Left", sideAxis);

        Vector3 rawMoveOffset = movementDirObject.transform.forward * forwardAxis + movementDirObject.transform.right * sideAxis;
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

    private void RotatePlayerInMoveDir() {        
        bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        if(!isMoving) { return; }

        Transform modelTran = animator.transform;
        float angle = MyMath.AngleWithDirection(modelTran.forward, movementDirObject.transform.forward, Vector3.up);

        if(Mathf.Abs(angle) < 5f) {
            modelTran.rotation = movementDirObject.transform.rotation;
            return;
        }

        int direction = (angle > 0) ? 1 : -1;
        modelTran.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime);
    }

    private void Gravity() {
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
        yaw += Input.GetAxis("Mouse X") * horzSensitivity * Time.deltaTime;
        pitch += Input.GetAxis("Mouse Y") * vertSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraRotateObject.transform.localRotation = Quaternion.Euler(pitch , yaw , 0f);
        movementDirObject.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        //transform.Rotate(Vector3.up * yaw);
    }

}
