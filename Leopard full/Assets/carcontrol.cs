using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class carcontrol : MonoBehaviour
{
    public float speed = 80000f;
    public float rotationSpeed = 15;
    private float movement = 0f;
    public WheelJoint2D backWheel;
    public WheelJoint2D frontWheel;
    public GameObject player;
    public bool ab = true;
    public Rigidbody2D rb;
    private float rotation = 0f;
    void Start()
    {

    }
    void OnCollisionEnter2D(Collision2D col)
    {


        while (ab == true)
        {
            player.transform.eulerAngles = new Vector3(0, 180, 0);

            ab = false;
        }



    }
    // Update is called once per frame
    void Update()
    {
        movement = -Input.GetAxisRaw("Vertical") * speed;
        rotation = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if(movement == 0f)
        {

            backWheel.useMotor = false;
            frontWheel.useMotor = false;
        }
        else
        {
            backWheel.useMotor = true;
            frontWheel.useMotor = true;
            JointMotor2D motor = new JointMotor2D { motorSpeed = movement, maxMotorTorque = 10000 };
            backWheel.motor = motor;
            frontWheel.motor = motor;
        }





        rb.AddTorque(-rotation * rotationSpeed * Time.fixedDeltaTime);












    }
}
