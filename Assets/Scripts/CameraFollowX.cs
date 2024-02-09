using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float start, end, bottom;

    private Vector3 tempVector3 = new Vector3();

    private void Start()
    {
        tempVector3.y = this.transform.position.y;
        tempVector3.z = this.transform.position.z;
    }

    private void Update()
    {
        if (target.position.x > start && target.position.x < end)
        {
            tempVector3.x = target.position.x;
            transform.position = tempVector3;
        }

        if (target.position.y > 8)
        {
            tempVector3.y = target.position.y;
        }
        else if (target.position.y < 8)
        {
            tempVector3.y = 2.7f;
        }
    }
}
