using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnityCarControllers : MonoBehaviour
{
    [Header("Transmission info")]
    public float Rpm;
    public int CurrentGear = 1;
    [Header("Car Properties")]
    public bool IsAccelerating = false;
    public bool IsReversing = false;        
    public float ForwardSpeed;
    public float motorTorque = 2000f;
    public bool IsBraking = false;
    public float brakeTorque = 2000f;
    public float MaxSpeed = 20f;
    public float ReverseSpeed = 10f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed;
    private float DeafaultSteeringRangeAtMaxSpeed = 10f;
    private float DriftSteeringRangeAtMaxSpeed = 40f;
    public float centreOfGravityOffset = -1f;
    float WheelAngle;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

    }


    private void Update()
    {
        // Get player input for acceleration and steering
        vInput = Input.GetAxis("Vertical"); // Forward/backward input
        hInput = Input.GetAxis("Horizontal"); // Steering input

        if (Input.GetKey(KeyCode.Space))
        {
            steeringRangeAtMaxSpeed = DriftSteeringRangeAtMaxSpeed;
        }
        else
            steeringRangeAtMaxSpeed = DeafaultSteeringRangeAtMaxSpeed;


        if (Input.GetKey(KeyCode.W))
        {
            if(ForwardSpeed > 0f)
            {
                Rpm += Time.deltaTime;
            }
        }
        else if(Input.GetKey(KeyCode.S))
        {
            IsReversing = true;
        }
        else
        {
            IsReversing = false;
            if (Rpm < 0f)
                Rpm = 0f;
            else
                Rpm -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
            IsBraking = true;
        else
            IsBraking = false;

        foreach(var wheel in wheels)//rotate wheels when steering
        {
            if(wheel.Steerable)
            {
                wheel.steerAngle = steeringRange * hInput;
                wheel.gameObject.transform.localEulerAngles = new Vector3(0f, hInput * steeringRange, 0f);
            }
        }

        Transmission();
    }
    // FixedUpdate is called at a fixed time interval 
    void FixedUpdate()
    {
        // Calculate current speed along the car's forward axis
        ForwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        float speedFactor = Mathf.InverseLerp(0, MaxSpeed, Mathf.Abs(ForwardSpeed)); // Normalized speed factor
        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);//build up to this var
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        bool isMovingForward = ForwardSpeed > 0f ? true : false;

        foreach (var wheel in wheels)
        {
            // Apply steering to wheels that support steering
            if (wheel.Steerable)
            {
                wheel.CurrentWheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isMovingForward)
            {
                // Apply torque to motorized wheels
                if (wheel.Motorized)
                {
                    wheel.CurrentWheelCollider.motorTorque = vInput * currentMotorTorque;
                }   
                // Release brakes when accelerating
                wheel.CurrentWheelCollider.brakeTorque = 0f;
            }
            else if(IsReversing)
            {
                // Apply torque to motorized wheels
                if (wheel.Motorized)
                {
                    wheel.CurrentWheelCollider.motorTorque = vInput * ReverseSpeed;//reverse speed
                }
                // Release brakes when accelerating
                wheel.CurrentWheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when coasting
                if (wheel.CurrentWheelCollider.motorTorque > 0f)
                    wheel.CurrentWheelCollider.motorTorque -= 0.5f * Time.deltaTime;
                else
                    wheel.CurrentWheelCollider.motorTorque = 0f;

                //wheel.CurrentWheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }
    /// <summary>
    /// changes max speed depending on RPMs.
    /// </summary>
    void Transmission()
    {
        if (Rpm >= 5f)
        {
            Rpm = 0;
            CurrentGear = 2;
            MaxSpeed = 30;
            Debug.Log("switched to 2 gear");
        }
    }
}
