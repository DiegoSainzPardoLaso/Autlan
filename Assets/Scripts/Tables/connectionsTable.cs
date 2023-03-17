using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class connectionsTable : MonoBehaviour
{
    [Header("Scripts")]
    public NodeManager nodeManager;

    [Header("Father")]
    public GameObject content;

    [Header("Prefabs")]
    public GameObject nodeRowPrefab;
    public GameObject nodeTransitionKeyPrefab;
    public GameObject nodeConnectionPrefab;
    public GameObject empty;

    [Header("Celdas")]
    public List<NodesTheirConnectionsAndKeys> nodeTableRow;
    public GameObject whiteCell;
    public GameObject greyCell;

    bool white = false;

    private void Start()
    {
        nodeTableRow = new List<NodesTheirConnectionsAndKeys>();
        
    }

    private void Update()
    {
        FillNodeNames();
        FillNodeConnections();
        DeleteNodes();

        FillNodeRow();
    }

    // HECHO
    void FillNodeNames()
    {
        int size = FindObjectsOfType<Node>().Length;
        // Cada vez que se añada un nuevo nodo esto se actualizara
        for (int i = 0; i < size; i++)
        {
            if (nodeManager.node[i].actualNode != null)
            {
                NodesTheirConnectionsAndKeys individualRow = new NodesTheirConnectionsAndKeys();
                
                individualRow.node  = nodeManager.node[i];

                bool alreadyIn = false ;

                for (int j = 0; j < nodeTableRow.Count; j++)
                {
                    if (nodeTableRow[j].node == individualRow.node)
                    {
                        alreadyIn = true;
                    }
                }

                if (!alreadyIn)
                {
                    nodeTableRow.Add(individualRow);
                }
            }
        }
    }

    // HECHO
    void FillNodeConnections()
    {
        int size = FindObjectsOfType<Node>().Length;

        // Recorro todos los nodos
        for (int i = 0; i < size; i++)
        {
            nodeTableRow[i].connections = new List<IndividualConnection>();

            nodeTableRow[i].connections = nodeManager.node[i].connectionsList;
        }
    }

    // HECHO
    void DeleteNodes()
    {
        for (int i = 0; i < nodeTableRow.Count; i++)
        {
            if (nodeTableRow[i].node == null)
            {
                Debug.Log("Borrando Nodo");
                nodeTableRow.RemoveAt(i);
            }
        }
    }

    void FillNodeRow()
    {
        if (nodeTableRow.Count > content.transform.childCount)
        {
            InstantiateCells();
            FillCellData();
        }
        else if (nodeTableRow.Count > content.transform.childCount)
        {
            // DeleteCells();
        }

        FillCellData();
    }

    void FillCellData()
    {
        for (int i = 0; i < nodeTableRow.Count; i++)
        {
            if (nodeTableRow[i].node.actualNode != null)
            {
                // Primero se reccoren los nodos y si hay mas de 1 nodo se le cambia el nombre a la celda que corresponda con el nombre del nodo.
                content.transform.GetChild(i).Find("Node (" + i + ")").transform.GetComponentInChildren<TMP_Text>().text = nodeTableRow[i].node.actualNode.GetComponent<TMP_Text>().text;

                if (nodeTableRow[i].connections != null)
                {
                    for (int j = 0; j < nodeTableRow[i].connections.Count; j++)
                    {
                        if (nodeTableRow[i].connections[j] != null)
                        {
                            // Si hay mas de una conexion, hay que instanciar nuevas celdas para la conexiones
                            if ((nodeTableRow[i].connections.Count) > 1)
                            {
                                if (nodeTableRow[i].connections != null && content.transform.GetChild(i).transform != null)
                                {
                                    GameObject fatherRow = content.transform.GetChild(i).gameObject;
                                    // Se le pasa la fila en la que se tiene que instanciar el objeto y el padre del objeto.
                                    InstantiateNewConnectionCells(j, fatherRow);
                                 
                                }
                            }
                            else
                            {
                                // Depues se comprueban las conexiones, si hay conexiones, se van iterando y se rellena la lista ("Hacer transiciones a parte").
                                content.transform.GetChild(i).Find("Connection (" + i + ")").transform.GetComponentInChildren<TMP_Text>().text = nodeTableRow[i].connections[j].actualConnection.GetComponent<Node>().actualNode.GetComponent<TMP_Text>().text;
                            }
                        }
                    }
                }
            }
        }
    }

    void InstantiateCells()
    {
        int size = nodeTableRow.Count - content.transform.childCount;
        
        for (int i = 0; i < size; i++)
        {
            GameObject nodeCell = Instantiate(nodeRowPrefab, this.transform.position, this.transform.rotation);
            nodeCell.transform.name = "Node (" + content.transform.childCount + ")";
            
            GameObject connectionCell = Instantiate(nodeConnectionPrefab, this.transform.position, this.transform.rotation);
            connectionCell.transform.name = "Connection (" + content.transform.childCount + ")";

            GameObject row = Instantiate(empty, this.transform.position, this.transform.rotation);
            row.transform.name = "Row (" + content.transform.childCount + ")";

            // Hace al nodo y su conexion hijos de la fila  
            nodeCell.transform.SetParent(row.transform);
            connectionCell.transform.SetParent(row.transform);

            row.transform.SetParent(content.transform);
            row.transform.localScale = Vector3.one;

            // La posicion en X es siempre la misma, pero para Y se recupera el ultimo "hijo" de Content y se le resta 28 (Numero magico) a la posicion del ultimo hijo
            row.GetComponent<RectTransform>().localPosition = new Vector2(258.2085f, content.transform.GetChild(content.transform.childCount - 2).transform.localPosition.y - 28);

            nodeCell.transform.localPosition = new Vector2(-192.4655f, nodeCell.transform.localPosition.y);
            connectionCell.transform.localPosition = new Vector2(65.31598f, connectionCell.transform.localPosition.y);
        }

    }

    void InstantiateNewConnectionCells(int i, GameObject fatherRow)
    {
        // El -1 es por que en Row siempre tiene que haber un celda resevada para el nodo
        int size = nodeTableRow[i].connections.Count - (fatherRow.transform.childCount - 1);
        Debug.Log(size);

        // Reducir el tamaño de los hijos que ya hay;
        for (int k = i; k < size; k++)
        {
            // Se redimensionan las celdas que ya existian
            if (fatherRow.transform.Find("Connection (" + k + ")").transform != null)
            {
                // Se obtiene el rect Transform del objeto
                RectTransform rectTransform = fatherRow.transform.Find("Connection (" + k + ")").Find("Text (TMP)").GetComponent<RectTransform>();

                // Se modifica la anchura
                Vector2 sizeDelta = new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height);

                // Se vuelve a asignar al objeto
                fatherRow.transform.Find("Connection (" + k + ")").Find("Text (TMP)").GetComponent<RectTransform>().sizeDelta = sizeDelta;
                
                // Se añaden nuevas celdas
                GameObject connectionCell = Instantiate(nodeRowPrefab, this.transform.position, this.transform.rotation);
                connectionCell.transform.name = "Connection (" + (fatherRow.transform.childCount - 1) + ")";

                connectionCell.transform.SetParent(fatherRow.transform);
                connectionCell.transform.localScale = Vector3.one;
                connectionCell.transform.localPosition = new Vector2(sizeDelta.x / 2, fatherRow.transform.Find("Connection (" + k + ")").localPosition.y);
                connectionCell.transform.GetComponentInChildren<TMP_Text>().text = nodeTableRow[i].connections[k + 1].actualConnection.GetComponent<Node>().actualNode.GetComponent<TMP_Text>().text;
            }
        }
    }

    void DeleteCells()
    {

    }

    public void MakeArrowVisible()
    {
        
    }

    
    public void MakeAsteriskVisible()
    {
        
    }

    
    public void RemoveCells(int index)
    {

    }

    // Para estructuras complejas como listas, o arrays es mejor utilizar una clase como
    // base en vez de una estructura, ya que al acceder a una estructura accedes a una copia
    // y no a la estructura original.
    [System.Serializable]
    public class NodesTheirConnectionsAndKeys
    {
        public Node node;
        // Almacena la conexion del nodo y sus llaves.
        public List<IndividualConnection> connections;
    }
}
