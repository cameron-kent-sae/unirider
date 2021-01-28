using System.Collections;
using System.Collections.Generic;
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
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, player.rotation, speed * Time.deltaTime);

        transform.LookAt(player);

        transform.position = Vector3.Slerp(transform.position, camLocation.position, (speed * (Vector3.Distance(transform.position, camLocation.position) / 4)) * Time.deltaTime);
    }
}
