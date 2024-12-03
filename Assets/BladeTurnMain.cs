using UnityEngine;

public class RotateBlades : MonoBehaviour
{
    public float rotationSpeed = 500f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
