using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : CSingletonMonobehaviour<ItemManager> {

    public List<GameObject> itemList;
    
	void Start ()
    {
        itemList = GameObject.FindGameObjectsWithTag("Item").ToList<GameObject>();

        SetItemIndex();
    }
	
    void SetItemIndex()
    {
        int idx = 0;
        foreach(GameObject item in itemList)
        {
            item.GetComponent<Item>().itemIndex = (byte)idx;
            idx++;
        }


    }

    public void ItemActive(byte item_index, byte player_index)
    {
        FindItem(item_index).ItemActive(UserManager.Instance.FindPlayer(player_index));
    }
    
    Item FindItem(byte index)
    {
       
        foreach (GameObject item in itemList)
        {
            if (item.GetComponent<Item>().itemIndex == index)
                return item.GetComponent<Item>();
        }

        return null;
    }

    public void ItemDisable(byte item_index)
    {
        FindItem(item_index).gameObject.SetActive(false);
    }
}
