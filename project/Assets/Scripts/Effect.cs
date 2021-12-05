using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    }
}
