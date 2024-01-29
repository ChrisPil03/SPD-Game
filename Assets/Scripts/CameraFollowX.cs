using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 tempVector3 = new Vector3();

    private void Start()
    {
        tempVector3.y = this.transform.position.y;
        tempVector3.z = this.transform.position.z;
    }

    private void Update()
    {
        if (target.position.x > 1 && target.position.x < 10)
        {
            tempVector3.x = target.position.x;
            transform.position = tempVector3;
        }
    }
}
