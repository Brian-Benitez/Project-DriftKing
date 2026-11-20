using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityCarControllers : MonoBehaviour
{
    [Header("Car Properties")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed;
    private float DeafaultSteeringRangeAtMaxSpeed = 10f;
    private float DriftSteeringRangeAtMaxSpeed = 40f;
    public float centreOfGravityOffset = -1f;

    float vInput;
    float hInput;
    public WheelSettingTest[] wheels;
    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

    }


    private void Update()
    {
        // Get player input for acceleration and steering
        vInput = Input.GetAxis("Vertical"); // Forward/backward input
        hInput = Input.GetAxis("Horizontal"); // Steering input
        if(Input.GetKey(KeyCode.Space))
        {
            steeringRangeAtMaxSpeed = DriftSteeringRangeAtMaxSpeed;
        }
        else
            steeringRangeAtMaxSpeed = DeafaultSteeringRangeAtMaxSpeed;
    }
    // FixedUpdate is called at a fixed time interval 
    void FixedUpdate()
    {
        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Determine if the player is accelerating or trying to reverse
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            // Apply steering to wheels that support steering
            if (wheel.Steerable)
            {
                wheel.CurrentWheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to motorized wheels
                if (wheel.Motorized)
                {
                    wheel.CurrentWheelCollider.motorTorque = vInput * currentMotorTorque;
                }   
                // Release brakes when accelerating
                wheel.CurrentWheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when reversing direction
                wheel.CurrentWheelCollider.motorTorque = 0f;
                wheel.CurrentWheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }
}
