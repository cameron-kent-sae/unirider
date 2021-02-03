using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player;
    public Transform camLocation;

    public float speed = 0.5f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Look at player
        if (player)
            transform.LookAt(player);

        // Follow Player
        if(camLocation)
        transform.position = Vector3.Slerp(transform.position, camLocation.position, (speed * (Vector3.Distance(transform.position, camLocation.position) / 4)) * Time.deltaTime);
    }
}
