using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : MonoBehaviour
{
    [Header("Scripts")]
    public InputManager inputManager;
    public NodeManager nodeManager;
    public MoveCamera moveCamera;

    [Header("Ventanas")]
    public GameObject popUpMenuWindowHoveringANode;
    public GameObject popUpMenuWindowNotHoveringANode;
    public GameObject popUpMenuWindowHoveringATransition;

    [Header("Prefabs")]
    public GameObject inputFieldPrefab;

    [Header("Crear Nodos")]
    public GameObject nodePrefab;
    public GameObject NodesFather;

    [Header("Datos")]
    public bool creatingNode;
    public bool activeState = false;
    public bool makeItEntryNode;
    public bool makeItExitNode;
    bool retargetLocation = false;
    bool addingNewTransition = false;

    void Update()
    {
        FollowMouse();
        
    }

    private void LateUpdate()
    {
        EnableDisableWindow();
    }

    void FollowMouse()
    {
        if (!activeState) 
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
        }

        if (retargetLocation) 
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
            retargetLocation = false;
        }
    }

    public void MakeItEntryNode()
    {
        nodeManager.entryPointNode = nodeManager.actualNode.GetComponent<Node>();
        makeItEntryNode = true;
    }

    public void MakeItEndNode()
    {
        if (nodeManager.entryPointNode == nodeManager.actualNode.GetComponent<Node>()) 
        {
            nodeManager.entryPointNode = null;
        }

        if (!nodeManager.exitNodes.Contains(nodeManager.actualNode.GetComponent<Node>()))
        {
            nodeManager.exitNodes.Add(nodeManager.actualNode.GetComponent<Node>());
            makeItExitNode = true;
        }
    }

    public void MakeConnection()
    {
        if (nodeManager.actualNode != null) 
        {
            nodeManager.makingConnection = true;  
        }
    }

    public void CreateNode()
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject node = Instantiate(nodePrefab, position, new Quaternion(0, 0, 0, 0));
        node.transform.parent = NodesFather.transform;

        node.GetComponentInChildren<TMP_Text>().text = "q" + nodeManager.node.Count;

        node.transform.name = "Node (" + nodeManager.node.Count + ")";
        
        nodeManager.node.Add(node.GetComponent<Node>());

        node.SetActive(true);
        moveCamera.SetParentingBackToNodes();
        moveCamera.RemoveNodesFromList();
        moveCamera.dragListFilled = false;
        creatingNode = true;
    }

    public void CreateNewTransitionKey()
    {
        GameObject newInputField = Instantiate(inputFieldPrefab, nodeManager.actualTransition.transform.position, nodeManager.actualTransition.transform.rotation);
        newInputField.transform.SetParent(nodeManager.actualTransition.transform.Find("Canvas").transform);
        newInputField.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        nodeManager.actualTransition.GetComponent<IndividualConnection>().inputFields.Add(newInputField.GetComponent<TMP_InputField>());
        nodeManager.actualTransition.GetComponent<IndividualConnection>().transitionKeys.Add(newInputField.GetComponentInChildren<TMP_InputField>().text);

        addingNewTransition = true;
    }

    public void DeleteNode()
    {
        if (!moveCamera.dragListFilled)
        {
            Destroy(nodeManager.actualNode);
            nodeManager.connectionIndex--;

            popUpMenuWindowHoveringANode.SetActive(false);
            popUpMenuWindowNotHoveringANode.SetActive(false);
            nodeManager.hoveringOverANode = false;
            activeState = false;
            moveCamera.dragListFilled = false;
        }
        else
        {
            int index = moveCamera.nodesDragSelection.Count - 1;

            foreach (Node node in moveCamera.nodesDragSelection)
            {
                if (node.actualNode != null) 
                {
                    Destroy(node.transform.gameObject);
                }
                
                nodeManager.connectionIndex--;

                popUpMenuWindowHoveringANode.SetActive(false);
                popUpMenuWindowNotHoveringANode.SetActive(false);
                nodeManager.hoveringOverANode = false;
                activeState = false;
            }

            moveCamera.SetParentingBackToNodes();
            moveCamera.RemoveNodesFromList();
            moveCamera.dragListFilled = false;

        }
        
    }

    void EnableDisableWindow()
    {
        if (activeState && inputManager.leftMouse && !inputManager.MouseOverUI() || nodeManager.makingConnection || creatingNode || makeItEntryNode || addingNewTransition || makeItExitNode)
        {
            moveCamera.SetParentingBackToNodes();
            moveCamera.RemoveNodesFromList();
            moveCamera.dragListFilled = false;
            nodeManager.hoveringOverANode = false;
            nodeManager.rightClickOverATransitionKey = false;
            popUpMenuWindowHoveringANode.SetActive(false);
            popUpMenuWindowNotHoveringANode.SetActive(false);
            popUpMenuWindowHoveringATransition.SetActive(false);
            activeState = false;
            creatingNode = false;
            makeItEntryNode = false;
            makeItExitNode = false;
            addingNewTransition = false;
            makeItExitNode = false;
        }

        if (activeState && inputManager.rightMouse)
        {
            retargetLocation = true;
            activeState = false;
        }


        // Cuando se abre la ventana estando sobre un nodo
        if (!activeState && inputManager.rightMouse && nodeManager.hoveringOverANode && !nodeManager.makingConnection)
        {
            popUpMenuWindowHoveringANode.SetActive(true);
            popUpMenuWindowNotHoveringANode.SetActive(false);
            popUpMenuWindowHoveringATransition.SetActive(false);
            activeState = true;
        }

        // Cuando la ventana se ha abierto mientras se estaba encima de un nodo y se pincha fuera del nodo
        if (!nodeManager.actuallyHoveringOverANode && inputManager.rightMouse)
        {
            // - Arregla Glitch Visual - //
            retargetLocation = true;
            FollowMouse();
            // *-----------------------* //

            popUpMenuWindowHoveringANode.SetActive(false);
            popUpMenuWindowHoveringATransition.SetActive(false);
            popUpMenuWindowNotHoveringANode.SetActive(true);
            nodeManager.hoveringOverANode = false;
            nodeManager.rightClickOverATransitionKey = false;
        }

        // Cuando se abre la ventana sin estar sobre un nodo
        if (!activeState && inputManager.rightMouse && !nodeManager.makingConnection)
        {      
            popUpMenuWindowHoveringANode.SetActive(false);
            popUpMenuWindowHoveringATransition.SetActive(false);
            popUpMenuWindowNotHoveringANode.SetActive(true);

            activeState = true;
        }

        // Cuando la ventana se ha abierto mientras se estaba fuera de un nodo ahora se esta encima
        if (nodeManager.actuallyHoveringOverANode && inputManager.rightMouse)
        {
            // - Arregla Glitch Visual - //
            retargetLocation = true;
            FollowMouse();
            // *-----------------------* //

            popUpMenuWindowHoveringANode.SetActive(true);
            popUpMenuWindowNotHoveringANode.SetActive(false);
            popUpMenuWindowHoveringATransition.SetActive(false);
            nodeManager.hoveringOverANode = true;
            
        }

        // Ventana para añadir transiciones
        if (nodeManager.rightClickOverATransitionKey)
        {
            popUpMenuWindowHoveringANode.SetActive(false);
            popUpMenuWindowNotHoveringANode.SetActive(false);
            popUpMenuWindowHoveringATransition.SetActive(true);
            activeState = true;
        }

    }
}
