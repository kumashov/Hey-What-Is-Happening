using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float TurnSpeedDegrees = 180.0f;

    private Animator _animator;
    private float _desiredRotationDegrees;

    public Animator Animator
    {
        get
        {
            if(_animator == null)
            {
                _animator = GetComponent<Animator>();
                if(_animator == null)
                    _animator = GetComponentInChildren<Animator>();
            }
            return _animator;
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if(Mathf.Abs(_desiredRotationDegrees) > 0.0f)
        {
            float thisFrameRotation = TurnSpeedDegrees * Time.deltaTime * Mathf.Sign(_desiredRotationDegrees);
            if(Mathf.Abs(thisFrameRotation) > Mathf.Abs(_desiredRotationDegrees))
            {
                thisFrameRotation = _desiredRotationDegrees;
                _desiredRotationDegrees = 0.0f;
            }
            else
            {
                _desiredRotationDegrees -= thisFrameRotation;
            }

            transform.RotateAround(transform.position, transform.up, thisFrameRotation);
        }
    }

    public void TurnToFace(Vector3 direction)
    {
        if(direction == Vector3.zero)
            return;
        Vector3 currFwd = transform.forward;
        currFwd.y = 0.0f;
        currFwd.Normalize();
        float currRotationDot = Vector3.Dot(currFwd, Vector3.forward);
        float currRotation = Mathf.Rad2Deg * Mathf.Acos(currRotationDot);
        if(Vector3.Dot(currFwd, Vector3.left) > 0.0f)
        {
            currRotation = -currRotation;
        }

        Vector3 targetFwd = direction;
        targetFwd.y = 0.0f;
        targetFwd.Normalize();
        float targetRotationDot = Vector3.Dot(targetFwd, Vector3.forward);
        float targetRotation = Mathf.Rad2Deg * Mathf.Acos(targetRotationDot);
        if(Vector3.Dot(targetFwd, Vector3.left) > 0.0f)
        {
            targetRotation = -targetRotation;
        }

        if(Mathf.Approximately(currRotation, targetRotation) == false)
        {
            _desiredRotationDegrees = targetRotation - currRotation;
            if(Mathf.Abs(_desiredRotationDegrees) > 180.0f)
            {
                if(_desiredRotationDegrees < 0.0f)
                {
                    _desiredRotationDegrees += 360.0f;
                }
                else if(_desiredRotationDegrees > 0.0f)
                {
                    _desiredRotationDegrees -= 360.0f;
                }
            }
        }
    }
}
