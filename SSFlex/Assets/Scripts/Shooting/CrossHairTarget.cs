using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    // Script von Felix
    // Purpose: Script to Determine where a Gun Should be Shooting

    [SerializeField] private Camera cam;

    private Ray ray;
    private RaycastHit hitInfo;


    // Update is called once per frame
    void Update()
    {
        // Check that Cam is not null
        if (cam == null)
            return;

        // Ray straight from Cam
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;

        // Check Position of Ray Point
        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            // IF ray didnt hit anything the Target Pos 1000m Straight from cam
            transform.position = ray.origin + ray.direction * 1000;
        }
    }
}
