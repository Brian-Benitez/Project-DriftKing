using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RBCarController : MonoBehaviour
{
    public Rigidbody CarRB;
    [Header("Accelerating Settings")]
    public float ForwardAccel, AccelMultipler, reverseAccel, SlowestSpeed, MaxSpeed, TurnStrength, GravityForce , DragGroundValue;
    private float speedInput, turnInput;
    private bool IsAcclerating = false;
    
    [Header("Braking Settings")]
    public bool IsBraking = false;
    public float BrakingPowerForSpeed;
    public float BrakingPowerForForwardAccel;
    public float SlowdownDecay;

    [Header("Gorund Raycasting Settings")]
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

        if (Input.GetKey(KeyCode.S))
            IsBraking = true;
        else
            IsBraking = false;

       

        if (Input.GetKey(KeyCode.S) || !IsAcclerating)//braking
        {
            if(IsBraking)
            {
                speedInput -= BrakingPowerForSpeed;
                ForwardAccel -= BrakingPowerForForwardAccel;

                if (ForwardAccel <= SlowestSpeed)
                    ForwardAccel = SlowestSpeed;

                if (speedInput < 0)
                    speedInput = 0;
            }
            else//for when player is not accelerating
            {
                if (speedInput < 0)
                    speedInput = 0;
                else
                {
                    speedInput -= SlowdownDecay;
                    ForwardAccel -= 0.05f;
                }

                if (ForwardAccel <= SlowestSpeed)
                    ForwardAccel = SlowestSpeed;
            }
            
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
        GripStrength();
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

    void GripStrength()
    {
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))//STALL ACCELERATION + TIGHTEN TURNING STREGNTH.
        {
            Debug.Log("LOOK AT ME!");
            if (CurrentGear == Gear.Third)//WHILE ON THIRD AND ACCELERATING
            {
                Debug.Log("LOOK AT ME!");
            }

            //MAKE ONE IF PLAYER LETS OFF THE GAS GIVE SOME GRIP BACK
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
