using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelsController : MonoBehaviour
{
    private Rigidbody rb;
    public bool FLWheel;
    public bool FRWheel;
    public bool RLWheel;
    public bool RRWheel;


    [Header("Suspension")]
    public float restLength;//where suspention rests
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springForce;
    private float damperForce;
    private float springVelocity;

    private Vector3 suspensionForce;
    public float SteerAngle;
    private float WheelAngle;
    private Vector3 WheelVelocityLS;//local space
    private float FX;
    private float FY;

    [Header("Wheel")]
    public float wheelRadius;
    public float SteeringTime;//How fast the wheels rotate

    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }


    private void Update()
    {
        WheelAngle = Mathf.Lerp(WheelAngle, SteerAngle, SteeringTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * WheelAngle);

        Debug.DrawRay(transform.position, -transform.up * (springLength + wheelRadius), Color.green);
    }
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            lastLength = springLength;

            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;

            WheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

            FX = Input.GetAxis("Vertical") * 2f;//sped SAME HERE
            FY = WheelVelocityLS.x * 2f;//NEEDS TO BE THE SAME


            rb.AddForceAtPosition(suspensionForce + (FX * transform.forward) + (FY * -transform.right), hit.point);

        }

    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.up * springLength, Color.green);
    }
}
