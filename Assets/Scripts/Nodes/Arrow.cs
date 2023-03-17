
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public IndividualConnection individualConnection;

    private List<Vector3> verticesList;
    private List<int> trianglesList;

    public float stemWidth; // 0.10
    public float tipLength; // 0.25
    public float tipWidth;  // 0.25
    public float stemLength;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Mesh mesh;


    void Start()
    {
        stemWidth = 0;
        tipLength = 0.3f;
        tipWidth  = 0.3f;

        meshFilter = this.transform.GetComponent<MeshFilter>();
        meshRenderer = this.transform.GetComponent<MeshRenderer>();
      
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    public void UpdatePositionAndRotation()
    {
        if (individualConnection.actualConnection != null && individualConnection.actualConnection.transform != this.transform.parent.parent.parent) 
        {
            this.transform.position = Vector3.Lerp(individualConnection.visualTrickeryPoint2.position, individualConnection.actualConnection.transform.Find("Bezier Visual Connection Point 3").transform.position, 0.5f);
            this.transform.LookAt(individualConnection.actualConnection.transform);
            this.transform.Rotate(0, -90, 0);
            stemLength = Vector3.Distance(this.transform.position, individualConnection.actualConnection.transform.position) - 0.75f;
            GenerateArrow();
        }
        else
        {
            stemLength = -0.80f;
            GenerateArrow();
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
