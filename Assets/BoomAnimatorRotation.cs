using UnityEngine;

public class RotateBoomAnimator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 1000000f; // Degrees per second

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}