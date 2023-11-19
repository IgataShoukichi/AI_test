using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public float speed = 5f;

    private Transform _transform;

    [SerializeField] Transform cameraTran;

    public Transform myTran;
    [System.NonSerialized] public Vector3 input;

    void FixedUpdate()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        var horizontalRotation = Quaternion.AngleAxis(cameraTran.eulerAngles.y, Vector3.up);

        var velocity = horizontalRotation * input;

        if (velocity.sqrMagnitude > 0.01f)
        {
            myTran.rotation = Quaternion.LookRotation(velocity);
        }

        if (velocity.sqrMagnitude > 1)
        {
            velocity = velocity.normalized;
        }

        rb.velocity = new Vector3(velocity.x * speed, rb.velocity.y, velocity.z * speed);
    }
}
