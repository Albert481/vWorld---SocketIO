using Project.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 1f;
    public float RotateSmoothTime = 0.1f;
    private float AngularVelocity = 0.0f;

    Vector3 direction;
    Coroutine co;
    private bool moving;

    [SerializeField]
    private NetworkIdentity networkIdentity;

    // Update is called once per frame
    void Update()
    {
        
        if (networkIdentity.IsControlling())
        {
            //checkMovement();

            if (Input.GetMouseButtonDown(0))
            {
                SetTargetPosition();
            }

        } else
        {
            Debug.Log("Not controlling");
        }

        
    }

    private void SetTargetPosition()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Debug.Log("target transform: " + raycastHit.transform.gameObject);
            // transform.position = Vector3.MoveTowards(transform.position, raycastHit.point, speed * Time.deltaTime);
            // Debug.Log("tP: " + raycastHit.point);
            direction = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z).normalized;

            if (!moving)
            {
                moving = true;
                co = StartCoroutine(MoveTo(transform, new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z), speed));
            } else
            {
                StopAllCoroutines();
                co = StartCoroutine(MoveTo(transform, new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z), speed));
            }
        }
        
    }

    IEnumerator MoveTo(Transform mover, Vector3 destination, float speed)
    {
        // This looks unsafe, but Unity uses
        // en epsilon when comparing vectors.

        while (mover.position != destination)
        {
            
            mover.position = Vector3.MoveTowards(
                mover.position,
                destination,
                speed * Time.deltaTime);

            var target_rot = Quaternion.LookRotation(destination - transform.position);
            var delta = Quaternion.Angle(transform.rotation, target_rot);
            if (delta > 0.0f)
            {
                var t = Mathf.SmoothDampAngle(delta, 0.0f, ref AngularVelocity, RotateSmoothTime);
                t = 1.0f - t / delta;
                transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, t);
            }

            // Wait a frame and move again.
            yield return null;
        }
        moving = false;
    }
}
