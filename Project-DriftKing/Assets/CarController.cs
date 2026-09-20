using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float Speed = 50;
    public float MaxSpeed = 15f;
    public float Drag = 0.98f;
    public float SteerAngle = 20;
    public float Traction = 1;

    [Header("Drift Settings")]
    public float DriftBoost;
    public bool IsDrifing = false;

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

        if (Input.GetKeyUp(KeyCode.Space) && IsDrifing)
        {
            StartCoroutine(SpeedBoost());
        }

        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.D))
            ActivateDriftSequence();
        else
            NormalDrivngControls();


            //traction
        UnityEngine.Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        UnityEngine.Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;
    }

    void ActivateDriftSequence()
    {
        SteerAngle = 10;
        Traction = 1;
        RightBackTrial.SetActive(true);
        LeftBackTrial.SetActive(true);

        CheckIfPlayerGetsBoost();
    }

    void CheckIfPlayerGetsBoost()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        {
            IsDrifing = true;
        }

    }
    IEnumerator SpeedBoost()
    {
        Speed = DriftBoost;
        yield return new WaitForSecondsRealtime(1f);
        UnityEngine.Debug.Log("done boost");
        Speed = MaxSpeed;
        IsDrifing = false;
    }
    void NormalDrivngControls()
    {
        SteerAngle = 3;
        Traction = 25;
        RightBackTrial.SetActive(false);
        LeftBackTrial.SetActive(false);
    }
}
