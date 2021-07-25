using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wirtten by Max
/// 
/// This Script is used in the Main Menu to let the Camera Follow Waypoints around the Map
/// </summary>
public class WaypointFollower : MonoBehaviour
{
    [SerializeField]
    private GameObject mStartPos;

    [SerializeField]
    private List<GameObject> mWaypoints;

    [SerializeField]
    private float mMaxMovementSpeed;
    private float mCurrentMovementSpeed;
    [SerializeField]
    private float mMovementccelaration;

    [SerializeField]
    private float mStartPosMoveSpeed;
    [SerializeField]
    private float mStartDamping;

    [SerializeField]
    private float mMaxDamping;
    private float mCurrentDamping;
    [SerializeField]
    private float mDampingModifier;

    [SerializeField]
    private float mStartDelay;

    private int mWaypointIdx;
    private bool mIsOnStartPos;
    private bool mGameStarted;

    private void Update()
    {
        if (mStartDelay <= 0)
            SetFollowBehaviour();
        else if(mIsOnStartPos)
            mStartDelay -= Time.deltaTime;

        if (mGameStarted && !mIsOnStartPos)
            MoveToStartPos();
    }

    public void StartGame()
    {
        mGameStarted = true;
    }

    /// <summary>
    /// Moves the Camera to the predetermined start position
    /// Add a dampong to its movement upon reaching the Start position
    /// </summary>
    private void MoveToStartPos()
    {
        if (transform.position != mStartPos.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, mStartPos.transform.position, mStartPosMoveSpeed * Time.deltaTime);

            mStartPosMoveSpeed -= mStartDamping * Time.deltaTime;
        }
        else if (transform.position == mStartPos.transform.position)
            mIsOnStartPos = true;
    }

    /// <summary>
    /// Lets the Camera accelerate after each waypoint
    /// </summary>
    private void SetFollowBehaviour()
    {
        if (mCurrentMovementSpeed < mMaxMovementSpeed)
            mCurrentMovementSpeed += mMovementccelaration * Time.deltaTime;

        if (mCurrentDamping < mMaxDamping)
            mCurrentDamping += mDampingModifier * Time.deltaTime;

        FollowWaypoints();
    }
      
    /// <summary>
    /// The next waypoint in line will be the move position to which the camera is moving
    /// The camera also rotates towards said point with an added Slerp
    /// </summary>
    private void FollowWaypoints()
    {
        if (transform.position != mWaypoints[mWaypointIdx].transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, mWaypoints[mWaypointIdx].transform.position, mCurrentMovementSpeed * Time.deltaTime);

            Quaternion rotation = Quaternion.LookRotation(mWaypoints[mWaypointIdx].transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * mCurrentDamping);
        }
        else 
        {
            mWaypointIdx++;
            mCurrentMovementSpeed /= 2;
        }

        if (mWaypointIdx >= mWaypoints.Count)
            mWaypointIdx = 0;
    }
}
