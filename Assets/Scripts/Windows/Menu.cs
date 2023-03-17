using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Scripts")]
    public InputManager inputManager;
    public MoveCamera moveCamera;
    public NodeManager nodeManager;
    public connectionsTable connections;

    [Header("Menus")]
    public GameObject connectionsMenu;

    [Header("Leyenda")]
    public TMP_Text entryNode;

    private void Update()
    {
        // Lo llamo desde aqui para que no se vea actualizandose el valor
        connections.MakeArrowVisible(); 
        MenuHandle();
        UpdateEntryNodeText();
    }

    void UpdateEntryNodeText()
    {
        if (nodeManager.entryPointNode != null)
        {
            entryNode.text = nodeManager.entryPointNode.actualNode.GetComponent<TMP_Text>().text;
        }
        else
        {
            entryNode.text = "";
        }
    }

    void MenuHandle()
    {
        if (inputManager.openMenu)
        {
            connectionsMenu.SetActive(true);
}
        else
        {
            connectionsMenu.SetActive(false);
        }
    }

}
