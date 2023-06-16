using UnityEngine;
using Mirror;

public class Mortar : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 30f;
    public GameObject ballPrefab; 
    public Transform firePoint; 
    public float launchSpeed = 20f;

    // limits for the Z-axis
    public float zMin = -3.44000006f;
    public float zMax = 0f;

    void Update()
    {
        if (!isLocalPlayer) return; // Only the local player should control the mortar

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move the mortar along the Z axis
        Vector3 move = new Vector3(0f, 0f, horizontal) * moveSpeed * Time.deltaTime;
        transform.Translate(move);

        // Check if the mortar has moved out of bounds and correct its position if it has
        float z = Mathf.Clamp(transform.position.z, zMin, zMax);
        transform.position = new Vector3(transform.position.x, transform.position.y, z);

        // Calculate the desired rotation based on the input
        float desiredRotation = transform.eulerAngles.z + vertical * rotateSpeed * Time.deltaTime;

        // Limit rotation to the 0-90 range
        desiredRotation = Mathf.Clamp(desiredRotation, 0, 90);

        // Apply the limited rotation to the mortar
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, desiredRotation));

        // Fire the bullet
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }
    
    [Command]
    void CmdFire()
    {
        GameObject ball = Instantiate(ballPrefab, firePoint.position, firePoint.rotation);

        // Adjust the direction to launch in the direction the firePoint is facing
        Vector3 direction = firePoint.right;
        
        Vector3 initialVelocity = direction * launchSpeed;

        // Apply the initial velocity to the ball's Rigidbody
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.velocity = initialVelocity;

        // Spawn the ball on the server
        NetworkServer.Spawn(ball);

        // Destroy the ball after 4 seconds
        Destroy(ball, 4f);
    }
}
