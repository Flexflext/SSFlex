using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mWaypoints;

    [SerializeField]
    private float mMovementSpeed;
    [SerializeField]
    private float mDamping;

    private int mWaypointIdx;

    private void Update()
    {
        if (transform.position != mWaypoints[mWaypointIdx].transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, mWaypoints[mWaypointIdx].transform.position, mMovementSpeed * Time.deltaTime);
            

            Quaternion rotation = Quaternion.LookRotation(mWaypoints[mWaypointIdx].transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * mDamping);
        }
        else mWaypointIdx++;

        if (mWaypointIdx >= mWaypoints.Count)
            mWaypointIdx = 0;

        Debug.Log(mWaypoints[mWaypointIdx]);
    }
}
