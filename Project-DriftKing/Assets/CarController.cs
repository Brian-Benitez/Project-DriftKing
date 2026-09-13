using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float Speed = 50;
    public float MaxSpeed = 15f;
    public float Drag = 0.98f;
    public float SteerAngle = 20;
    public float Traction = 1;

    [Header("Trials")]
    public GameObject RightBackTrial;
    public GameObject LeftBackTrial;    

    private Vector3 MoveForce;
    void Update()
    {
        //Moving
        MoveForce += transform.forward * Speed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        //Steering
        float steeringInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steeringInput * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        if(Input.GetKey(KeyCode.Space))
            ActivateDriftSequence();
        else
            NormalDrivngControls();

            //traction
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;
    }

    void ActivateDriftSequence()
    {
        SteerAngle = 20;
        Traction = 1;
        RightBackTrial.SetActive(true);
        LeftBackTrial.SetActive(true);
    }

    void NormalDrivngControls()
    {
        SteerAngle = 10;
        Traction = 10;
        RightBackTrial.SetActive(false);
        LeftBackTrial.SetActive(false);
    }
}
