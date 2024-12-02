using UnityEngine;

public class BikeCollision : MonoBehaviour
{
    public float moveBackDistance = 1.0f;
    public float moveBackSpeed = 5.0f;

    private Vector3 moveBackDirection;
    private bool isColliding = false;

    private BicycleVehicle bikeVehicle;

    void Start()
    {
        bikeVehicle = GetComponent<BicycleVehicle>();
    }

    void Update()
    {
        if (isColliding)
        {
            
            transform.position += moveBackDirection * moveBackSpeed * Time.deltaTime;

            // Stop backward movement after covering the distance
            moveBackDistance -= moveBackSpeed * Time.deltaTime;
            if (moveBackDistance <= 0)
            {
                isColliding = false;
                moveBackDistance = 1.0f; // Reset distance for next collision
            }
        }
    }

    public void OnChildCollisionEnter(Collision collision)
    {
 
        Vector3 collisionNormal = collision.contacts[0].normal;
        moveBackDirection = -collisionNormal.normalized;

        isColliding = true;

        
        bikeVehicle.enabled = false;

    }

    public void OnChildCollisionExit(Collision collision)
    {
  
        isColliding = false;

        
        bikeVehicle.enabled = true;
   
    }
}
