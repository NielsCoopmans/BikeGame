using UnityEngine;

public class ChildCollisionHandler : MonoBehaviour
{
    public BikeCollision parentScript; // Reference to the parent's script

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Child collision detected with: " + collision.gameObject.name);
        if (parentScript != null)
        {
            parentScript.OnChildCollisionEnter(collision);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (parentScript != null)
        {
            parentScript.OnChildCollisionExit(collision);
        }
    }
}
