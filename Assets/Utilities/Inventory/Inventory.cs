using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    List<Item> items = new List<Item>();

    public void Clear() {
        items.Clear();
        PrintInventory();
    }
    
    public void Add(InventoryItem newItem) {
        foreach (Item item in items) {
            if (item.itemId == newItem.itemId) {
                item.amount += newItem.amount;
                PrintInventory();
                return;
            }
        }
        
        items.Add(new Item(newItem));
        PrintInventory();
    }

    public Item Find(string itemId) {
        foreach(Item item in items) {
            if(item.itemId == itemId) {
                return item;
            }
        }

        return null;
    }

    public string PrintInventory() {
        string str = "";
        foreach(Item item in items) {
            str += item.ToString() + "\n";
        }
        //Debug.Log(str);

        return str;
    }

    public class Item {
        public string itemId = string.Empty;
        public int amount = 0;

        public Item(InventoryItem inventoryItem) {
            itemId = inventoryItem.itemId;
            amount = inventoryItem.amount;
        }

        public override string ToString() {
            string str = itemId + ": " + amount;
            return str;
        }
    }
}
