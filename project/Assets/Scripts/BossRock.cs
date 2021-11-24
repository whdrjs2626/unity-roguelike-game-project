using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPower());
        StartCoroutine(GainPowerTimer());
        Destroy(gameObject, 7);
    }

    IEnumerator GainPowerTimer() {
        yield return new WaitForSeconds(2.0f);
        isShoot = true;
    }

    IEnumerator GainPower() {
        while(!isShoot) {
            angularPower += 50f;
            scaleValue += 0.01f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
    }

}
