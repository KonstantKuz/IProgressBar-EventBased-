using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRot : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Sin(Time.time) * 25f, Vector3.up);
    }
}
