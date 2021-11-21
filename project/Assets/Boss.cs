using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : enemy
{
    public GameObject missile_prefab;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec; // 쳐다보는 곳
    Vector3 tauntVec; // 내려찍을 곳
    bool isLook;
    
    void Awake() {        
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mats = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>();

        isLook = true;
        nav.isStopped = true;
        StartCoroutine("Think");
        //Debug.Log("Think...");
    }
    
    void Update() {
        if(isLook) {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
            Debug.Log("look");
        }
        else {
            nav.SetDestination(tauntVec);
        }
        if(HP <= 0) {
            isLook = false;
            StopAllCoroutines();
            return;
        }
    }

    IEnumerator Think() {
        Debug.Log("Think");
        yield return new WaitForSeconds(0.1f);
        int rand = Random.Range(0, 5);
        switch(rand) {
            case 0:
            case 1:
                StartCoroutine("missileShoot");
                break;
            case 2:
            case 3:
                StartCoroutine("rockShoot");
                break;
            case 4:
                StartCoroutine("Taunt");
                break;
        }
    }

    IEnumerator missileShoot() {
        ani.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject missileA = Instantiate(missile_prefab, missilePortA.position, missilePortA.rotation);
        Bullet bossMissileA = missileA.GetComponent<Bullet>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject missileB = Instantiate(missile_prefab, missilePortB.position, missilePortB.rotation);
        Bullet bossMissileB = missileB.GetComponent<Bullet>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.5f);
        StartCoroutine("Think");
    }

    IEnumerator rockShoot() {
        isLook = false;
        ani.SetTrigger("doRock");
        Instantiate(bullet_prefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine("Think");
    }

    IEnumerator Taunt() {
        isLook = false;
        nav.isStopped = false;
        tauntVec = target.position + lookVec;
        boxCollider.enabled = false; // 점프 뛸 때 플레이어를 밀지 않도록
        ani.SetTrigger("doTaunt");
        
        yield return new WaitForSeconds(2f);
        meleeArea.enabled = true;
        
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        boxCollider.enabled = true;
        nav.isStopped = true;
        isLook = true;
        StartCoroutine("Think");
    }
}
