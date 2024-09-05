using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget;
    public Transform lookAtTarget;
    public bool hasDelay = false;
    public float delaySpeed = 3.0f;

    void Start()
    {
    }

    void LateUpdate()
    {
        // 만일, follow Target이 있다면...
        if (followTarget != null)
        {
            // 만일, hasDelay 값이 true라면...
            if (hasDelay)
            {
                // Lerp를 이용해서 살짝 느리게 쫓아간다.
                transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * delaySpeed);
            }
            // 그렇지 않다면...
            else
            {
                // 해당 타겟을 향해 이동한다.
                transform.position = followTarget.position;
            }
        }
        // 만일, LookAt Target이 있다면...
        if (lookAtTarget != null)
        {
            // 해당 타겟을 항상 바라보도록 회전한다.
            //Vector3 dir = lookAtTarget.position - transform.position;
            //transform.forward = dir;
            transform.rotation = lookAtTarget.rotation;
        }
    }

    public void SetCameraLookAtTarget(Transform target)
    {
        lookAtTarget = target;
    }

    public void SetCameraFollowTarget(Transform target)
    {
        followTarget = target;
    }
}
