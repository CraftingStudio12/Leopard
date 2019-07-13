using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{

    private const float Speed = 10;

    private Rigidbody rb;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        this.rb.AddForce(Speed * x, 0, Speed * z);
    }
}