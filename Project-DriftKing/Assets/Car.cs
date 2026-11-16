using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public List<GameObject> WheelsPos;
    public LayerMask Ground;

    // Update is called once per frame
    void Update()
    {
        foreach(var wheel in WheelsPos)
        {
            RaycastHit hit;
            Vector3 orgin = wheel.transform.position;

            if(Physics.Raycast(orgin, -transform.up, out hit ,Ground))
            {
                Debug.Log("hit");
                Debug.DrawLine(orgin, -transform.up, Color.red);
            }
        }
    }
}
