﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour,IGameManager {

	public ManagerStatus status { get; private set; }
    private Dictionary<string, int> _items;
    public string equippedItem { get; private set; }
    private NetworkService _network;

    public List<string>GetItemList()
    {
        //return a List of all the Dictionary keys
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    //Returns how many of that item are in inventory
    public int GetItemCount(string name)
    {
        if (_items.ContainsKey(name))
        {
            return _items[name];
        }
        return 0;
    }
    public void Startup(NetworkService service)
    {
        Debug.Log("Inventory manager starting...");
        _network = service;
        //_items = new Dictionary<string, int>();
        UpdateData(new Dictionary<string, int>());
        status = ManagerStatus.Started;
    }
   
    public void UpdateData(Dictionary<string, int> items)
    {
        _items = items;
    }
    //getter for save game code to access data
    public Dictionary<string, int> GetData()
    {
        return _items;
    }
   
    private void DisplayItems()
    {
        string itemDisplay = "Items: ";
        foreach (KeyValuePair<string, int>item in _items)
        {
            itemDisplay += item.Key + "("+item.Value+")";
        }
        Debug.Log(itemDisplay);
    }
    public void AddItem(string name)
    {
        if (_items.ContainsKey(name))
        {
            _items[name] += 1;
        } else
        {
            _items[name] = 1;
        }
        DisplayItems();
    }

    public bool EquipItem(string name)
    {
        //check that inventory has the item and it isn't already equipped
        if (_items.ContainsKey(name) && equippedItem != name)
        {
            equippedItem = name;
            Debug.Log("Equipped " + name);
            return true;
        }
        equippedItem = null;
        Debug.Log("Unequipped");
        return false;
    }

    public bool ConsumeItem(string name)
    {
        if (_items.ContainsKey(name))
        {
            _items[name]--;
            if (_items[name] == 0)
            {
                _items.Remove(name);
            }
        }
        else {
            Debug.Log("cannot consume " + name);
            return false;
        }

        DisplayItems();
        return true;
    }
}
