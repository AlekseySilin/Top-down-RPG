using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicUI : MonoBehaviour {

	void OnGUI()
    {
        int posX = 10;
        int posY = 10;
        int width = 100;
        int height = 30;
        int buffer = 10;
        List<string> itemList = Managers.Inventory.GetItemList();
        if (itemList.Count == 0)
        {
            GUI.Box(new Rect(posX, posY, width, height), "No items");
        }
        foreach (string item in itemList)
        {
            int count = Managers.Inventory.GetItemCount(item);
            Texture2D image = Resources.Load<Texture2D>("Icons/" + item);
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("(" + count + ")", image));
            posX += width + buffer; //Shift sideways each time through the loop
        }
        string equipped = Managers.Inventory.equippedItem;
        if (equipped != null)
        {
            posX = Screen.width - (width + buffer);
            Texture2D image = Resources.Load("Icons/" + equipped) as Texture2D;
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped", image));
        }
        posX = 10;
        posY += height + buffer;
        // loop through all items to make buttons
        foreach (string item in itemList)
        {
            //Run this code if button clicked
            if (GUI.Button(new Rect(posX,posY,width,height),"Equip " + item))
            {
                Managers.Inventory.EquipItem(item);
            }
            if (item == "health")
            {
                if(GUI.Button(new Rect(posX,posY+height+buffer,width,height),"Use Health"))
                {
                    Managers.Inventory.ConsumeItem("health");
                    Managers.Player.ChangeHealth(25);
                }
            }
            posX += width + buffer;
        }
    }
}
