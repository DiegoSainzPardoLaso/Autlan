using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveCamera : MonoBehaviour
{
    [Header("Scripts")]
    public InputManager InputManager;
    public NodeManager nodeManager;
    public PopUpWindow popUpWindow;

    [Header("Objetos Externos")]
    public Camera camera;
    public RectTransform box;
    public GameObject multipleNodesSelection;

    [Header("Parametros")]
    public float zoomSpeed = 10f;    
    public float minZoom = 5f;     
    public float maxZoom = 30f;

    [Header("Datos")]
    public Vector2 selectionBoxStartPosition;
    public Vector2 selectionBoxEndPosition;
    public List<Node> nodesDragSelection = new List<Node>();
    public bool dragListFilled;
    public bool draggingSelectionObject;
    public bool draggingBox;
    bool activateDraggingBox;

    Rect selectionBox;
    Vector3 dragOrigin;
    bool once = true;
    public float scrollInput;
    public Vector3 difference;



    private void Start()
    {
        DrawVisualSelectionBox(Vector2.zero, Vector2.zero);
    }

    private void Update()
    {
        PanCamera();
        ZoomCamera();
        SelectionBox();
        MoveMultipleNodes();
    }

    void PanCamera()
    {

        if (!InputManager.panning)
        {
            once = true;
        }

        if (InputManager.panning && once)
        {
            dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
            once = false;
        }

        if (InputManager.panning)
        {
            Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.mousePosition);

            camera.transform.position += new Vector3(difference.x, difference.y, 0);
        }

    }

    void ZoomCamera()
    {
        if (!InputManager.openMenu)
        {
            scrollInput = Input.GetAxis("Mouse ScrollWheel");
            float newFieldOfView = camera.orthographicSize - scrollInput * zoomSpeed;
            newFieldOfView = Mathf.Clamp(newFieldOfView, minZoom, maxZoom);
            camera.orthographicSize = newFieldOfView;
        }
    }

    void SelectionBox()
    {
        // Click Izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            selectionBoxStartPosition = Input.mousePosition;
            selectionBox = new Rect();
        }

        if (!popUpWindow.activeState && !nodeManager.draggingNode)
        {
            // Arrastre del raton
            if (Input.GetMouseButton(0) && !draggingSelectionObject && !nodeManager.draggingNode)
            {
                selectionBoxEndPosition = Input.mousePosition;
                DrawVisualSelectionBox(selectionBoxStartPosition, selectionBoxEndPosition);
                DrawLogicalSelectionBox();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                SelectNodes();
                selectionBoxStartPosition = Vector2.zero;
                selectionBoxEndPosition = Vector2.zero;
                DrawVisualSelectionBox(selectionBoxStartPosition, selectionBoxEndPosition);
                draggingBox = false;
            }
        }
        else
        {
            DrawVisualSelectionBox(Vector2.zero, Vector2.zero);
        }

        // Click fuera de la seleccion o cualquiera de las acciones de la ventana
        if ((Input.GetMouseButtonDown(0) && !InputManager.MouseOverUI()) || popUpWindow.creatingNode ||
            popUpWindow.makeItEntryNode || nodeManager.makingConnection)
        {
            SetParentingBackToNodes();
            RemoveNodesFromList();
            dragListFilled = false;
        }

        if (Input.GetMouseButtonDown(0) && !nodeManager.actuallyHoveringOverANode) { activateDraggingBox = true; }

        if (activateDraggingBox) { draggingBox = true; }
        
        if (draggingSelectionObject) { DrawVisualSelectionBox(Vector2.zero, Vector2.zero); }

        if (dragListFilled && Input.GetMouseButtonDown(0) && nodeManager.actuallyHoveringOverANode) { draggingSelectionObject = true; }

        if (Input.GetMouseButtonUp(0)) { draggingSelectionObject = false; draggingBox = false; activateDraggingBox = false; }

    }

    public void SetParentingBackToNodes()
    {
        foreach(Node node in nodesDragSelection)
        {
            if (node.actualNode != null) 
            { 
                node.transform.SetParent(nodeManager.transform);
            }
        }
    }

    void DrawVisualSelectionBox(Vector2 startPos, Vector2 endPos)
    {
        Vector2 boxCenter = (startPos + endPos) / 2;
        box.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(startPos.x - endPos.x), Mathf.Abs(startPos.y - endPos.y));
        box.sizeDelta = boxSize;
    }

    void DrawLogicalSelectionBox()
    {
        if (!draggingSelectionObject)
        {
            // Arrastrando de derecha a Izquierda
            if (Input.mousePosition.x < selectionBoxStartPosition.x)
            {
                selectionBox.xMin = Input.mousePosition.x;
                selectionBox.xMax = selectionBoxStartPosition.x;
            }
            else
            {
                selectionBox.xMin = selectionBoxStartPosition.x;
                selectionBox.xMax = Input.mousePosition.x;
            }

            // Arrastrando de arriba a abajo
            if (Input.mousePosition.y < selectionBoxStartPosition.y)
            {
                selectionBox.yMin = Input.mousePosition.y;
                selectionBox.yMax = selectionBoxStartPosition.y;
            }
            else
            {
                selectionBox.yMin = selectionBoxStartPosition.y;
                selectionBox.yMax = Input.mousePosition.y;
            }


            // Efecto visual, solo para cambiar el color del nodo cuando se pase la caja por encima aunque no se seleccione
            foreach (Node node in nodeManager.node)
            {
                if (selectionBox.Contains(camera.WorldToScreenPoint(node.transform.position)))
                {
                    // Naranja
                    node.transform.GetComponent<SpriteRenderer>().material.color = new Color(1, 0.905f, 0.438f);
                }
                else if (!dragListFilled && node != nodeManager.entryPointNode && !nodeManager.draggingNode && !nodeManager.exitNodes.Contains(node))
                {
                    // Blanco
                    node.transform.GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1);
                }
                else if (!dragListFilled && node == nodeManager.entryPointNode && !nodeManager.draggingNode && !nodeManager.exitNodes.Contains(node))
                {
                    node.transform.GetComponent<SpriteRenderer>().material = node.entryMaterial;
                }
                
                if (nodeManager.exitNodes.Contains(node) && !selectionBox.Contains(camera.WorldToScreenPoint(node.transform.position)) && !dragListFilled)
                {
                    node.transform.GetComponent<SpriteRenderer>().material = node.exitMaterial;
                }
            }
        }
        
    }

    void SelectNodes()
    {
        foreach (Node node in nodeManager.node)
        {
            if (node.actualNode != null && selectionBox.Contains(camera.WorldToScreenPoint(node.transform.position)))
            {
                if (!nodesDragSelection.Contains(node))
                {
                    node.transform.GetComponent<SpriteRenderer>().material.color = new Color(1, 0.905f, 0.438f);
                    nodesDragSelection.Add(node);

                    dragListFilled = true;
                }
            }
        }
        if (dragListFilled)
        {
            foreach (Node node in nodesDragSelection)
            {
                if (node.actualNode != null) 
                {
                    node.transform.SetParent(multipleNodesSelection.transform);
                }
            }
        }
    }

    void MoveMultipleNodes()
    {
        // Click
        if (Input.GetMouseButtonDown(0))
        {
            if (dragListFilled)
            {
                difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)multipleNodesSelection.transform.position;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (dragListFilled)
            {
                multipleNodesSelection.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)difference;
            }
        }


    }

    public void RemoveNodesFromList()
    {
        nodesDragSelection.RemoveRange(0, nodesDragSelection.Count);
        dragListFilled = false;
    }
}
