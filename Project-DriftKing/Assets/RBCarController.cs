using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RBCarController : MonoBehaviour
{
    public Rigidbody CarRB;
    public float ForwardAccel, AccelMultipler, reverseAccel, SlowestSpeed, MaxSpeed, TurnStrength, GravityForce , DragGroundValue;
    public float SlowdownDecay;
    private float speedInput, turnInput;

    public bool IsAcclerating = false;
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


        if (Input.GetKeyUp(KeyCode.W))
            IsAcclerating = false;

        if (Input.GetKey(KeyCode.S) || !IsAcclerating)//braking
        {
            if (speedInput < 0)
                speedInput = 0;
            else
            {
                speedInput -= SlowdownDecay;
                ForwardAccel -= 0.05f;
                Debug.Log("slow car down");
            }

            if (ForwardAccel <= SlowestSpeed)
                ForwardAccel = 5f;
            /*
            if (Input.GetAxis("Vertical") < 0)//reversing
            {
                speedInput = Input.GetAxis("Vertical") * reverseAccel;
            }
            */
        }
 
        if (Input.GetKey(KeyCode.W))//accelerating
        {
            IsAcclerating = true;
            speedInput = Input.GetAxis("Vertical") * ForwardAccel * 1000;

            if (ForwardAccel >= MaxSpeed)
                ForwardAccel = MaxSpeed;
            else
                ForwardAccel += AccelMultipler;//0.0003f;
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
       
        /*
        if(CurrentGear == Gear.Second && RPM <= MaxRPMForFirstGear)
        {
            CurrentGear = Gear.First;
            GearText.text = 1.ToString();
            ForwardAccel = 5;
        }
        */

        if (ForwardAccel >= 5.5f && CurrentGear != Gear.Second)
        {
            CurrentGear = Gear.Second;
            GearText.text = 2.ToString();
            AccelMultipler = 0.0005f;
        }
        if (ForwardAccel >= 6.5f && CurrentGear != Gear.Third)
        {
            CurrentGear = Gear.Third;
            GearText.text = 3.ToString();
            AccelMultipler = 0.0006f;
        }
    }
}
