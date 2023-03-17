using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HandleViewportDropDownMenu : MonoBehaviour
{

    public TMP_Dropdown dropDownMenu;
    public NodeManager  nodeManager;
    public InputManager inputManager;

    public List<string> exitNodesNames = new List<string>();


    void Update()
    {
        UpdateDropDownList();
        RemoveNodesFromList();
    }

    void UpdateDropDownList()
    {
        if (nodeManager.exitNodes != null)
        {
            for (int i = 0; i < nodeManager.exitNodes.Count; i++) 
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData("-");

                if (nodeManager.exitNodes[i].GetComponent<Node>().actualNode != null)
                {
                    data = new TMP_Dropdown.OptionData(nodeManager.exitNodes[i].GetComponent<Node>().actualNode.GetComponent<TMP_Text>().text);
                            
                    if (!exitNodesNames.Contains(nodeManager.exitNodes[i].GetComponent<Node>().actualNode.GetComponent<TMP_Text>().text))
                    {
                        exitNodesNames.Add(nodeManager.exitNodes[i].GetComponent<Node>().actualNode.GetComponent<TMP_Text>().text);
                        dropDownMenu.options.Add(data);
                    }
                }
            }
        }
    }

    void RemoveNodesFromList()
    {
        if (!inputManager.MouseOverUI() && !dropDownMenu.IsExpanded) 
        {
            dropDownMenu.ClearOptions();
            exitNodesNames.Clear();
        }
    }
}
