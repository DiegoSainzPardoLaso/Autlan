using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    public InputManager inputManager;
    public PopUpWindow popUpWindow;
    NodeManager nodeManager;
    MoveCamera moveCamera;

    [Header("Nodos")]
    public GameObject actualNode;
    public List<IndividualConnection> connectionsList;
    public float inputFieldVerticalOffset = 0.2f;

    [Header("Line Renderer")]
    public GameObject connectionsFather;
    public GameObject connectionPrefab;
    // Lista de los prefabs con los mesh renderer y mesh filter
    // No asignar nada, se ira rellenando "sola"

    [Header("Parametros del Line Renderer")]
    public Color color;
    private SpriteRenderer renderer;

    [Header("Materiales")]
    public Material defaultNodeMaterial;    
    public Material entryMaterial;
    public Material exitMaterial;

    [Header("Flecha Estetica")]
    public float stemLength;
    public GameObject meshFilterObject;

    List<Vector3> verticesList;
    List<int>     trianglesList;
    float stemWidth = 0.079f; // 0.10
    float tipLength = 0.35f; // 0.25
    float tipWidth  = 0.35f; // 0.25
    Mesh mesh;

    // Arrastre multiple //
    bool once;
    Vector3 mouseOffset;

    Vector2 difference;
    GameObject thisObject;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        popUpWindow  = FindObjectOfType<PopUpWindow>();
        nodeManager  = FindObjectOfType<NodeManager>();
        moveCamera   = FindObjectOfType<MoveCamera>();

        thisObject = this.transform.gameObject;
        // Flecha Estetica // 
        mesh = new Mesh();
        meshFilterObject.GetComponent<MeshFilter>().mesh = mesh;
        stemWidth = 0.079f;
        tipLength = 0.3f;
        tipWidth  = 0.3f; 
        // --------------- //

        renderer = GetComponent<SpriteRenderer>();
    
        
    }

    private void Update()
    {
        int size = connectionsFather.transform.childCount;

        for (int i = 0; i < size; i++) 
        {
            connectionsFather.transform.GetChild(i).GetComponent<IndividualConnection>().UpdateArrow();
            connectionsFather.transform.GetChild(i).GetComponent<IndividualConnection>().HandleTransitionText();
            connectionsFather.transform.GetChild(i).GetComponent<IndividualConnection>().DestroyConnection();

        }

        for (int i = 0; i < connectionsList.Count; i++)
        {
            if (thisObject != null)
            {
                if (connectionsList.ElementAt(i).actualConnection == null)
                {
                    connectionsList.Remove(connectionsList.ElementAt(i));
                }
            }
        }

    }

    private void OnMouseDown()
    {
        if (!moveCamera.dragListFilled)
        {
            difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        }
    }

    private void OnMouseDrag()
    {
        if (!moveCamera.dragListFilled)
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
        }
    }

    private void OnMouseUp()
    {
        once = false; 
    }

    private void OnMouseEnter()
    {
        if (!moveCamera.dragListFilled && !moveCamera.draggingBox)
        {
            renderer.material.color = color;
            nodeManager.hoveringOverANode = true;
        }
    }

    private void OnMouseExit()
    {
        if (!moveCamera.dragListFilled)
        {
            renderer.material.color = Color.white;
        }

        if (!popUpWindow.activeState)
        {
            nodeManager.hoveringOverANode = false;
        }
    }

    // Solo es estetico, no hace nada
    public void DrawArrowToMouse()
    {
        if (thisObject != null && nodeManager.makingConnection && nodeManager.actualNode.transform.name.Equals(thisObject.transform.name))
        {
            meshFilterObject.GetComponent<MeshRenderer>().enabled = true;

            Vector3 trace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            trace.z = 0;

            stemLength = Vector3.Distance(trace, this.transform.position) - 0.2f;
            GenerateArrow();

            Vector3 direction = trace - this.transform.position;

            meshFilterObject.transform.up = direction;
            meshFilterObject.transform.Rotate(0, 0, 90);

            if (inputManager.rightMouse)
            {
                nodeManager.makingConnection = false;
                Debug.Log("Conexion Cancelada");
            }

        }
        else if (thisObject != null)
        {
            meshFilterObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }


    public void UpdateNodeMaterials()
    {
        if (thisObject != null && nodeManager.entryPointNode != null && nodeManager.entryPointNode.transform.name.Equals(this.transform.name) &&
            !moveCamera.dragListFilled && !moveCamera.draggingBox)
        {
            // Verde
            this.transform.GetComponent<SpriteRenderer>().material = entryMaterial;
        }
        else if (thisObject != null && nodeManager.exitNodes != null && nodeManager.exitNodes.Contains(this) && !moveCamera.draggingBox && !moveCamera.dragListFilled)
        {
            // Rojizo
            this.transform.GetComponent<SpriteRenderer>().material = exitMaterial;
        }
        else if (thisObject != null && !nodeManager.actuallyHoveringOverANode && !nodeManager.hoveringOverANode && !moveCamera.dragListFilled && !moveCamera.draggingBox)
        {
            // Blanco
            this.transform.GetComponent<SpriteRenderer>().material = defaultNodeMaterial;
        }
        else if (nodeManager.hoveredNode != null && nodeManager.hoveredNode.transform.parent.GetComponent<Node>() == this && moveCamera.draggingBox && moveCamera.dragListFilled) 
        {
            renderer.material.color = color;
        }
    }
    

    public void UpdateNodeEntryExitStatus()
    {
        int index = 0;
        if (this == nodeManager.entryPointNode) 
        {
            for (int i = 0; i < nodeManager.exitNodes.Count; i++)
            {
                if (nodeManager.exitNodes[i] == this)
                {
                    nodeManager.exitNodes.RemoveAt(i);
                }
                index++;
            }
        }
    }


    public void ConnectionHandling()
    {
        if (thisObject != null && nodeManager.conectNode && nodeManager.nextNode != null && nodeManager.actualNode.transform.name.Equals(this.transform.name))
        {
            bool isAlreadyIn = false;

            for (int i = 0; i < connectionsFather.transform.childCount; i++)
            {
                if (nodeManager.nextNode.transform.name.Equals(connectionsFather.transform.GetChild(i).GetComponent<IndividualConnection>().actualConnection.transform.name))
                {
                    isAlreadyIn = true; 
                }
            }

            if (!isAlreadyIn)
            {
                GameObject connection = Instantiate(connectionPrefab, connectionsFather.transform.position, connectionsFather.transform.rotation);
                connection.transform.parent = connectionsFather.transform;
                connection.transform.name = "Connection (" + nodeManager.connectionIndex + ")";
                connection.GetComponent<IndividualConnection>().actualConnection = nodeManager.nextNode;
                connectionsList.Add(connection.GetComponent<IndividualConnection>());

                nodeManager.connectionIndex++;
                nodeManager.makingConnection = false;
                nodeManager.conectNode = false;
            }
            else
            {
                nodeManager.makingConnection = false;
                nodeManager.conectNode = false;
            }
        }
    }


    public void GenerateArrow()
    {
        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        Vector3 stemOrigin = Vector3.zero;
        float stemHalfWidth = stemWidth / 2;

        // Vertices de la cola de la flecha
        verticesList.Add(stemOrigin + (stemHalfWidth * Vector3.down));
        verticesList.Add(stemOrigin + (stemHalfWidth * Vector3.up));
        verticesList.Add(verticesList[0] + (stemLength * Vector3.right));
        verticesList.Add(verticesList[1] + (stemLength * Vector3.right));

        // Triangulos de la cola de la flecha
        trianglesList.Add(0);
        trianglesList.Add(1);
        trianglesList.Add(3);

        trianglesList.Add(0);
        trianglesList.Add(3);
        trianglesList.Add(2);

        // Punta de la flecha
        Vector3 tipOrigin = stemLength * Vector3.right;
        float tipHalfWidth = tipWidth / 2;

        // Vertices de la flecha
        verticesList.Add(tipOrigin + (tipHalfWidth * Vector3.up));
        verticesList.Add(tipOrigin + (tipHalfWidth * Vector3.down));
        verticesList.Add(tipOrigin + (tipLength * Vector3.right));

        // Triangulos de flecha
        trianglesList.Add(4);
        trianglesList.Add(6);
        trianglesList.Add(5);

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
    }

}
