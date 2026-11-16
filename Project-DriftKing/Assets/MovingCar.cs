using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCar : MonoBehaviour
{
    public WheelsController[] Wheels;

    [Header("Car Specs")]
    public float WheelBase;
    public float RearTrack;
    public float TurnRadius;

    [Header("Inputs")]
    public float SteerInput;

    public float ackermannAngleLeft;
    public float ackermannAngleRight;
    public Rigidbody CarRB;
    // Update is called once per frame
    void Update()
    {
        SteerInput = Input.GetAxis("Horizontal");

        if (SteerInput > 0)//is turning right
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * SteerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * SteerInput;
        }
        else if (SteerInput < 0)//is left
        {
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * SteerInput;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * SteerInput;
        }
        else
        {
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0; 
        }

        foreach(WheelsController wheel in Wheels)
        {
            if (wheel.FLWheel)
                wheel.SteerAngle = ackermannAngleLeft;
            if (wheel.FRWheel)
                wheel.SteerAngle = ackermannAngleRight;
        }
    }
}
