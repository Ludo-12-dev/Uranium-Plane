using UnityEngine;

public class DrawSurroundingSeaPlane : MonoBehaviour
{
    [Header("Plane")]
    public GameObject planeObject;

    [Header("Circle Settings")]
    public float circleRadius = 2.5f;
    public Vector2 circleCenter = Vector2.zero;

    [Header("Texture Settings")]
    public int textureSize = 1024;

    [Header("Colors")]
    public Color insideCircleColor = Color.green;
    public Color seaBlueColor = new Color(0.0f, 0.45f, 0.75f, 1f);

    private void Start()
    {
        DrawSeaAroundCircle();
    }

    private void DrawSeaAroundCircle()
    {
        if (planeObject == null)
        {
            Debug.LogError("Plane Object not assigned.");
            return;
        }

        MeshRenderer meshRenderer = planeObject.GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("Assigned object has no MeshRenderer.");
            return;
        }

        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / (textureSize - 1);
                float v = (float)y / (textureSize - 1);

                // Unity Plane = 10x10 units
                float planeX = (u - 0.5f) * 10f;
                float planeZ = (v - 0.5f) * 10f;

                Vector2 point = new Vector2(planeX, planeZ);

                if (Vector2.Distance(point, circleCenter) <= circleRadius)
                    texture.SetPixel(x, y, insideCircleColor);
                else
                    texture.SetPixel(x, y, seaBlueColor);
            }
        }

        texture.Apply();

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.mainTexture = texture;

        meshRenderer.material = mat;
    }
}