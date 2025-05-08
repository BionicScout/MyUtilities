using UnityEngine;

[CreateAssetMenu(fileName = "SO_InventoryItem" , menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject {
    public string itemId = string.Empty;
    public int amount = 0;

    public override string ToString() {
        string str = itemId + ": " + amount;
        return str;
    }
}
