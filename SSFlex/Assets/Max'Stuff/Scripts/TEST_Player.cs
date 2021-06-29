using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TEST_Player : MonoBehaviour
{
    [SerializeField]
    private float mMovementSpeed;
    [SerializeField, Range(0.0f, 0.5f)]
    private float mWalkSmoothTime;

    [SerializeField, Range(0.0f, 0.5f)]
    private float mMouseSmoothTime;
    private float mCameraExtend = 50f;
    private float mCameraPitch;

    [SerializeField]
    private float mMouseSensitivity;

    [SerializeField]
    private GameObject mCamPivot;

    private Vector2 mCurrentDirection;
    private Vector2 mCurrentVelocity;
    private Vector2 mCurrentMouseDelta;
    private Vector2 mCurrentMouseDeltaVelocity;
    private Vector3 mVelocity;

    Rigidbody Rb;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();

        mCurrentDirection = new Vector2();
        mCurrentVelocity = new Vector2();
        mCurrentMouseDelta = new Vector2();
        mCurrentMouseDeltaVelocity = new Vector2();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        Move();
        MouseLook();
    }

    private void MouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        mCurrentMouseDelta = Vector2.SmoothDamp(mCurrentMouseDelta, targetMouseDelta, ref mCurrentMouseDeltaVelocity, mMouseSmoothTime);

        mCameraPitch -= mCurrentMouseDelta.y * mMouseSensitivity;
        mCameraPitch = Mathf.Clamp(mCameraPitch, -mCameraExtend, mCameraExtend);

        mCamPivot.transform.localEulerAngles = Vector3.right * mCameraPitch;

        transform.Rotate(Vector3.up * mCurrentMouseDelta.x * mMouseSensitivity);
    }

    private void Move()
    {
        Vector2 targetdir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetdir.Normalize();
        mCurrentDirection = Vector2.SmoothDamp(mCurrentDirection, targetdir, ref mCurrentVelocity, mWalkSmoothTime);

        mVelocity = (transform.forward * mCurrentDirection.y + transform.right * mCurrentDirection.x) * mMovementSpeed + Vector3.up * Time.deltaTime;

        Rb.MovePosition(transform.position + mVelocity * Time.deltaTime);
    }
}
