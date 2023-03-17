using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class IndividualConnection : MonoBehaviour
{
    public Arrow arrow;
    public Vector3 actualPosition;
    // Separo la posicion de la conexion del objeto conectado para acceder a ello directamente
    public Vector3 connectionPosition;
    public Node node;
    public GameObject actualConnection;

    public List<string> transitionKeys;
    public List<TMP_InputField> inputFields;

    Vector3 point2;
    public Transform visualTrickeryPoint2;

    [Header("Line Renderer")]
    LineRenderer lineRenderer;
    public float vertexCount = 12f;

    private void Start()
    {
        // La inicializo con una "Conexion" para que no de un error
     

        lineRenderer = this.transform.GetComponent<LineRenderer>();
    }

    public void UpdateArrow()
    {
        if (actualConnection != null) 
        {
            actualPosition = this.transform.position;
            connectionPosition = actualConnection.transform.position;

            RotateInstancedConnections();
        }

        GenerateCuadraticBezierCurve();
        arrow.UpdatePositionAndRotation();
    }

    public void HandleTransitionText()
    {
 
        if (actualConnection != null)
        {
            // Conexion a si mismo
            if (actualConnection == this.transform.parent.parent.gameObject)
            {
                // Añadir a esta lista al crear nueva transicion con el boton
                for (int i = 0; i < inputFields.Count; i++)
                {
                    // El ((i + 2) / 2) es para que se vea bien en pantalla
                    float difference = ((float)(i) / 3.0f);
                    inputFields[i].transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1f - difference, 0);
                    inputFields[i].transform.up = Vector3.zero;
                }
            }
            else
            {
                Vector3 midpoint = Vector3.Lerp(visualTrickeryPoint2.position, actualConnection.transform.Find("Bezier Visual Connection Point 3").transform.position, 0.5f);

                if (this.transform.rotation.z < 0.7f)
                {
                    // Parte completamente grafica.
                    for (int i = 0; i < inputFields.Count; i++)
                    {
                        float difference = ((float)i) / 3.0f;
                        inputFields[i].transform.position = new Vector3(midpoint.x, midpoint.y - 0.2f + difference, 0);
                        inputFields[i].transform.up = Vector3.zero;
                    }
                }
                else
                {
                    // Parte completamente grafica.
                    for (int i = 0; i < inputFields.Count; i++)
                    {
                        float difference = ((float)i) / 3.0f;
                        inputFields[i].transform.position = new Vector3(midpoint.x, midpoint.y - 0.2f - difference, 0);
                        inputFields[i].transform.up = Vector3.zero;
                    }
                }
            }

            // Asignacion de la llave.
            for (int i = 0; i < inputFields.Count; i++)
            { 
                transitionKeys[i] = inputFields[i].text;   
            }
        }
    }

    void RotateInstancedConnections()
    {
        Vector3 direction = connectionPosition - this.transform.position;

        this.transform.up = direction;
        this.transform.Rotate(0, 0, 90);
        //this.transform.rotation = new Quaternion(0, 0, this.transform.rotation.z, this.transform.rotation.w);
    }

    public void DestroyConnection()
    {
        if (actualConnection == null)
        {
            Destroy(this.transform.gameObject);
        }
    }

    public void GenerateCuadraticBezierCurve()
    {
        if (actualConnection != null)
        {
            Vector3 point1 = this.transform.position;
            Vector3 point3 = actualConnection.transform.position;
            List<Vector3> pointList = new List<Vector3>();

            if (actualConnection.transform.Find("Bezier Visual Connection Point 3") != null && actualConnection.transform != this.transform.parent.parent)
            {
                point2 = Vector3.Lerp(visualTrickeryPoint2.position, actualConnection.transform.Find("Bezier Visual Connection Point 3").transform.position, 0.5f);

                for (float ratio = 0; ratio <= 1; ratio += 1 / vertexCount)
                {

                    Vector3 tangent1 = Vector3.Lerp(point1, point2, ratio);
                    Vector3 tangent2 = Vector3.Lerp(point2, point3, ratio);
                    Vector3 curvePoint = Vector3.Lerp(tangent1, tangent2, ratio);

                    pointList.Add(curvePoint);

                }

                lineRenderer.positionCount = pointList.Count;
                lineRenderer.SetPositions(pointList.ToArray());
            }
        }
    }
}
