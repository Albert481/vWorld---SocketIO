using Project.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField]
    private NetworkIdentity networkIdentity;

    // Update is called once per frame
    void Update()
    {
        
        if (networkIdentity.IsControlling())
        {
            checkMovement();
        } else
        {
            Debug.Log("Not controlling");
        }

        
    }

    private void FixedUpdate()
    {
        
    }

    private void checkMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // transform.position += new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
        
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);
        }
    }
}
