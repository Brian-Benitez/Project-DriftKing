using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class RBCarController : MonoBehaviour
{
    public Rigidbody CarRB;
    public float ForwardAccel, reverseAccel, MaxSpeed, TurnStrength, GravityForce , DragGroundValue;

    private float speedInput, turnInput;

    private bool Grounded;

    public LayerMask WhatIsGround;
    public float GroundRayLength = 0.5f;
    public Transform GroundRayPoint;

    public enum Gear
    {
        First,
        Second,
        Third,
    }
    [Header("Transmission Settings")]
    public Gear CurrentGear;
    public float RPM;
    public float MaxRPMForFirstGear;
    public float MaxRPMForSecondGear;

    public TextMeshProUGUI GearText;

    public Transform LeftFrontWheel, RightFrontWheel;
    public float MaxWheelTurn;

    void Start()
    {
        CarRB.transform.parent = null;
        GearText.text = 1.ToString();  
    }


    void Update()
    {
        speedInput = 0;
        if(Input.GetAxis("Vertical") > 0 )
        {
            speedInput = Input.GetAxis("Vertical") * ForwardAccel * 1000;
            RPM++;
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000;
        }
        else
        {
            if (RPM <= 0)
                RPM = 0;
            else
                RPM--;
        }

        turnInput = Input.GetAxis("Horizontal");

        if(Grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * TurnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }

        LeftFrontWheel.localRotation = Quaternion.Euler(LeftFrontWheel.localRotation.eulerAngles.x, (turnInput * MaxWheelTurn), LeftFrontWheel.localRotation.eulerAngles.z);
        RightFrontWheel.localRotation = Quaternion.Euler(RightFrontWheel.localRotation.eulerAngles.x, (turnInput * MaxWheelTurn), RightFrontWheel.localRotation.eulerAngles.z);
        transform.position = CarRB.transform.position;

        Transmission();
    }

    private void FixedUpdate()
    {
        Grounded = false;
        RaycastHit hit;
        if(Physics.Raycast(GroundRayPoint.position, -transform.up, out hit, GroundRayLength, WhatIsGround))
        {
            Grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if(Grounded)
        {

            CarRB.drag = DragGroundValue;
            if (Mathf.Abs(speedInput) > 0)
            {
                CarRB.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            CarRB.drag = 0.1f;
            CarRB.AddForce(Vector3.up * -GravityForce * 100f);
        }
    }

    void Transmission()
    {
        if(CurrentGear == Gear.Second && RPM <= MaxRPMForFirstGear)
        {
            CurrentGear = Gear.First;
            GearText.text = 1.ToString();
            ForwardAccel = 5;
        }
        if (RPM >= MaxRPMForFirstGear)
        {
            CurrentGear = Gear.Second;//idea, have max accelrate here and slowly build up to it, so it feels a bit more natural
            GearText.text = 2.ToString();
            ForwardAccel = 8;
        }
        if (RPM >= MaxRPMForSecondGear)
        {
            CurrentGear = Gear.Third;
            GearText.text = 3.ToString();
            ForwardAccel = 12;
        }


    }
}
