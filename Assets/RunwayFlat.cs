using UnityEngine;

public class ForceRunwayFlat : MonoBehaviour
{
    public Transform[] runwayPlanes;
    public float fixedY = 0f;

    private void Start()
    {
        foreach (Transform t in runwayPlanes)
        {
            if (t == null) continue;

            Vector3 p = t.position;
            p.y = fixedY;
            t.position = p;

            t.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}