using UnityEngine;

public class RotateBladesBack : MonoBehaviour
{
    public float rotationSpeed = 500f;

    void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}

