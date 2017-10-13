using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimInCircle : MonoBehaviour
{
    public float MinCircleRadius = 16.0f;
    public float MaxCircleRadius = 16.0f;
    public float MinCircleRadiusRepickDelay = 2.0f;
    public float MaxCircleRadiusRepickDelay = 8.0f;
    public Transform CircleCentre;

    public float SwimSpeed = 1.0f;

    private Vector3 _circleCenterPoint;
    private float _currCircleRadius;
    private float _targetCircleRadius;

    private float _currAngle = 0.0f;
    private float _circleRadiusRepickDelay = 0.0f;

    // Use this for initialization
    void Start()
    {
        if(CircleCentre == null)
            _circleCenterPoint = transform.position;
        else
            _circleCenterPoint = CircleCentre.position;

        _currCircleRadius = _targetCircleRadius = MinCircleRadius;

        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach(var animator in animators)
        {
            animator.Play("Swim", 0, Random.Range(0.0f, 1.0f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Approximately(MinCircleRadius, MaxCircleRadius) == false)
        {
            if(Mathf.Approximately(_currCircleRadius, _targetCircleRadius))
            {
                _circleRadiusRepickDelay -= Time.deltaTime;

                if(_circleRadiusRepickDelay <= 0.0f)
                {
                    _targetCircleRadius = Random.Range(MinCircleRadius, MaxCircleRadius);
                    _circleRadiusRepickDelay = Random.Range(MinCircleRadiusRepickDelay, MaxCircleRadiusRepickDelay);
                }
            }
            else
            {
                float changeThisFrame = Mathf.Sign(_targetCircleRadius - _currCircleRadius) * Time.deltaTime;
                if(Mathf.Abs(changeThisFrame) > Mathf.Abs(_targetCircleRadius - _currCircleRadius))
                {
                    _currCircleRadius = _targetCircleRadius;
                }
                else
                {
                    _currCircleRadius += changeThisFrame;
                }
            }
            
        }

        float circleCircumference = 2 * Mathf.PI * _currCircleRadius;

        float anglePerSecond = 360.0f * (SwimSpeed / circleCircumference);
        _currAngle += Time.deltaTime * anglePerSecond;
        Vector3 targetPos = _circleCenterPoint + Quaternion.Euler(new Vector3(0.0f, _currAngle, 0.0f)) * (Vector3.forward * _currCircleRadius);
        transform.LookAt(targetPos);
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        transform.position += (moveDirection * SwimSpeed * Time.deltaTime);
    }
}
