using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCarController : MonoBehaviour
{
    public List<WheelCollider> SteeringWheels;
    public List<WheelCollider> ThrottleWheels;

    public float StrenghtCoeffecient;
    public float MaxTurnRadius = 20f;
    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(WheelCollider wheel in ThrottleWheels)
        {
            wheel.motorTorque = StrenghtCoeffecient * Time.deltaTime * Input.GetAxis("Vertical");
        }
        foreach(WheelCollider wheel in SteeringWheels)
        {
            wheel.steerAngle = MaxTurnRadius * Input.GetAxis("Horizontal");
            wheel.gameObject.transform.localEulerAngles = new Vector3(0f, Input.GetAxis("Horizontal") * MaxTurnRadius, 0f);
        }
    }
}
