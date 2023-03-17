using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    [Header("Scripts")]
    public InputManager inputManager;
    public List<Node> node;
    public connectionsTable connectionS;
    public MoveCamera moveCamera;

    [Header("Nodos")]
    public GameObject hoveredNode;
    public GameObject actualNode;
    public GameObject nextNode;

    [Header("Logica de Nodos")]
    public Node entryPointNode;
    public List<Node> exitNodes;
    public GameObject actualTransition;

    [Header("Objetos Externos")]
    public GameObject cameraObject;

    [Header("Datos")]
    public bool draggingNode;
    public bool makingConnection;
    public bool conectNode = false;
    public bool hoveringOverANode = false;
    public bool actuallyHoveringOverANode = false;
    public bool rightClickOverATransitionKey = false;

    public int connectionIndex = 0;

    public int menuDeletionIndex;

    bool clickStartedOnANode;

    private void Awake()
    {
        int size = this.transform.childCount;
      
        for (int i = 0; i < size; i++) 
        {
            node.Add(this.transform.GetChild(i).GetComponent<Node>());
        }
    }
    private void Update()
    {
        NodeSelection();
        DraggingNode();
    }

    private void LateUpdate()
    {
        NodeUpdate();
        UpdateMaterial();
        DeleteNodeFromList();
    }

    void DeleteNodeFromList()
    {
        for (int i = 0; i < node.Count; i++)
        {
            if (node.ElementAt(i) == null)
            {
                if (i < node.Count - 1)
                {
                    RenameNodes(i + 1);
                }
           
                node.RemoveAt(i);
            }
        }  
    }

    public void RenameNodes(int index)
    {
        for (int i = index; i < node.Count; i++)
        {
            if (node.ElementAt(i) != null) 
            {
                if ((i - 1) > -1)
                {
                    Debug.Log("Ranaming");
                    node.ElementAt(i).transform.name = "Node (" + (i - 1) + ")";
                    node.ElementAt(i).transform.gameObject.GetComponentInChildren<TMP_Text>().text = "q" + (i - 1);
                }
            }
        }
    }

    void NodeSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000)) 
        {
 
            Debug.DrawRay(cameraObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - cameraObject.transform.position, Color.green);

            actuallyHoveringOverANode = true;
            hoveredNode = hitInfo.transform.gameObject;

            if (hitInfo.transform.tag.Equals("Node") && inputManager.rightMouse) 
            {
                actualNode = hitInfo.transform.parent.gameObject;
            }

            if (hitInfo.transform.tag.Equals("Node") && inputManager.leftMouse && makingConnection)
            {
                nextNode = hitInfo.transform.parent.gameObject;
                conectNode = true;
                makingConnection = false;
            }

            if (hitInfo.transform.tag.Equals("transition") && inputManager.rightMouse)
            {
                // Asi se accede a la conexion actual
                actualTransition = hitInfo.transform.parent.parent.gameObject;
                rightClickOverATransitionKey = true;
            }

            if (hitInfo.transform.tag.Equals("Node") && Input.GetMouseButtonDown(0))
            {
                clickStartedOnANode = true;
            }

        }
        else
        {
            actuallyHoveringOverANode = false;
            hoveredNode = null;
        }

        Debug.DrawRay(cameraObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - cameraObject.transform.position, Color.red);
    }

    void DraggingNode()
    {
        if (hoveringOverANode && inputManager.leftMouse && clickStartedOnANode || actuallyHoveringOverANode && Input.GetMouseButtonDown(0))
        {
            draggingNode = true;
        }
        if (!inputManager.leftMouse)
        {
            draggingNode = false;
            clickStartedOnANode = false;
        }
    }

    void NodeUpdate()
    {
        for (int i = 0; i < node.Count; i++)
        {
            node[i].ConnectionHandling();
            node[i].DrawArrowToMouse();
            node[i].UpdateNodeEntryExitStatus();
        }
    }

    void UpdateMaterial()
    {
        for (int i = 0; i < node.Count; i++)
        {
            node[i].UpdateNodeMaterials();
        }
    }
}
