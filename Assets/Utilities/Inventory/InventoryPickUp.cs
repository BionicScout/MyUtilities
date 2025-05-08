using UnityEngine;

public class InventoryPickUp : MonoBehaviour {
    public InventoryItem inventoryItem;

    [Header("Float")]
    public bool floating = false;
    public float floatLerpSpeed = 0.75f;
    public float floatOffset = 0.3f;

    private Vector3 startFloatPoint;
    private Vector3 endFloatPoint;
    private float floatLerpTime = 0f;
    private bool floatUp = true;

    [Header("Rotation")]
    public bool rotating = false;
    public float rotateAngleSpeed = 40f;

    private void Start() {
        startFloatPoint = transform.position;
        endFloatPoint = startFloatPoint + Vector3.up * floatOffset;
    }

    private void OnTriggerEnter(Collider other) {
        Inventory inventory = other.transform.GetComponent<Inventory>();

        if(inventory != null) {
            inventory.Add(inventoryItem);
            Destroy(gameObject);
        }
    }

    private void Update() {

        //Floating
        if(floatUp) {
            floatLerpTime += floatLerpSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(startFloatPoint , endFloatPoint , floatLerpTime);

            if(floatLerpTime >= 1) {
                floatLerpTime = 1;
                floatUp = false;
            }
        }
        else {
            floatLerpTime -= floatLerpSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(startFloatPoint , endFloatPoint , floatLerpTime);

            if(floatLerpTime <= 0) {
                floatLerpTime = 0;
                floatUp = true;
            }
        }


        //Rotation
        if(rotating) {
            transform.Rotate(0, rotateAngleSpeed * Time.deltaTime, 0);
        }
    }
}
