using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mWaypoints;

    [SerializeField]
    private float mMaxMovementSpeed;
    private float mCurrentMovementSpeed;
    [SerializeField]
    private float mMovementccelaration;


    [SerializeField]
    private float mMaxDamping;
    private float mCurrentDamping;
    [SerializeField]
    private float mDampingModifier;

    [SerializeField]
    private float mStartDelay;

    private int mWaypointIdx;

    private void Update()
    {
        if (mStartDelay <= 0)
        {
            if (mCurrentMovementSpeed < mMaxMovementSpeed)
                mCurrentMovementSpeed += mMovementccelaration * Time.deltaTime;

            if(mCurrentDamping < mMaxDamping)
                mCurrentDamping += mDampingModifier * Time.deltaTime;

            FollowWaypoints();
        }
        else
            mStartDelay -= Time.deltaTime;   
    }
      
    private void FollowWaypoints()
    {
        if (transform.position != mWaypoints[mWaypointIdx].transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, mWaypoints[mWaypointIdx].transform.position, mCurrentMovementSpeed * Time.deltaTime);

            Quaternion rotation = Quaternion.LookRotation(mWaypoints[mWaypointIdx].transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * mCurrentDamping);
        }
        else mWaypointIdx++;

        if (mWaypointIdx >= mWaypoints.Count)
            mWaypointIdx = 0;
    }
}
