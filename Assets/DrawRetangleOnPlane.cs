using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DrawRectangleOnPlane : MonoBehaviour
{
    [Header("Main Grey Rectangle")]
    public float length = 8f;
    public float halfWidth = 0.25f;
    public Color rectangleColor = Color.gray;

    [Header("White Median Mini Rectangles")]
    public int numberOfMiniRectangles = 6;
    public float miniRectangleLength = 0.6f;
    public float miniRectangleHalfWidth = 0.035f;
    public Color miniRectangleColor = Color.white;

    [Header("World Height")]
    public float runwayWorldY = 0.01f;
    public float miniWorldY = 0.02f;

    [Header("Materials")]
    public Material mat;
    public Material mat2;

    private void Start()
    {
        DrawRunway();
    }

    private void DrawRunway()
    {
        transform.rotation = Quaternion.identity;

        DrawRectangle(
            "Grey Rectangle",
            length,
            halfWidth,
            runwayWorldY,
            rectangleColor
        );

        DrawMedianMiniRectangles();
    }

    private void DrawMedianMiniRectangles()
    {
        float spacing = length / numberOfMiniRectangles;
        float startX = -length * 0.5f + spacing * 0.5f;

        for (int i = 0; i < numberOfMiniRectangles; i++)
        {
            float centerX = startX + i * spacing;

            GameObject miniRect = new GameObject("White Median Mini Rectangle " + (i + 1));
            miniRect.transform.SetParent(transform, false);

            miniRect.AddComponent<MeshFilter>();
            miniRect.AddComponent<MeshRenderer>();

            miniRect.transform.localPosition = Vector3.zero;
            miniRect.transform.localRotation = Quaternion.identity;
            miniRect.transform.localScale = Vector3.one;

            Mesh mesh = CreateRectangleMesh(
                miniRectangleLength,
                miniRectangleHalfWidth,
                miniWorldY,
                centerX
            );

            miniRect.GetComponent<MeshFilter>().mesh = mesh;

            if (mat != null)
            {
                Material whiteMat = new Material(mat);
                whiteMat.color = miniRectangleColor;
                miniRect.GetComponent<MeshRenderer>().material = whiteMat;
            }
        }
    }

    private void DrawRectangle(
        string meshName,
        float rectLength,
        float rectHalfWidth,
        float rectWorldY,
        Color color
    )
    {
        Mesh mesh = CreateRectangleMesh(
            rectLength,
            rectHalfWidth,
            rectWorldY,
            0f
        );

        mesh.name = meshName;

        GetComponent<MeshFilter>().mesh = mesh;

        if (mat2 != null)
        {
            Material greyMat = new Material(mat2);
            greyMat.color = color;
            GetComponent<MeshRenderer>().material = greyMat;
        }
    }

    private Mesh CreateRectangleMesh(
        float rectLength,
        float rectHalfWidth,
        float worldY,
        float centerX
    )
    {
        Mesh mesh = new Mesh();

        float halfLength = rectLength * 0.5f;

        float localY = worldY - transform.position.y;

        Vector3[] vertices =
        {
            new Vector3(centerX - halfLength, localY, -rectHalfWidth),
            new Vector3(centerX + halfLength, localY, -rectHalfWidth),
            new Vector3(centerX - halfLength, localY,  rectHalfWidth),
            new Vector3(centerX + halfLength, localY,  rectHalfWidth)
        };

        int[] triangles =
        {
            0, 2, 1,
            2, 3, 1
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}