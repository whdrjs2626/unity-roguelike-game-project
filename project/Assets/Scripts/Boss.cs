using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : enemy
{
    public GameObject missile_prefab;
    public Transform missilePortA;
    public Transform missilePortB;
    public GameObject manager_object;
    public GameManager manager;

    Vector3 lookVec; // 쳐다보는 곳
    Vector3 tauntVec; // 내려찍을 곳
    bool isLook;

    GameObject temp;
    public RectTransform bossHPGroup; // 보스 체력
    public RectTransform bossHPBar; // 보스 체력 바


    void Awake() {
        deadFlag = true;
        temp = GameObject.Find("Boss Group");
        bossHPGroup = temp.GetComponent<RectTransform>();
        temp = GameObject.Find("Boss Real HP");
        bossHPBar = temp.GetComponent<RectTransform>();
        target = GameObject.Find("Player").transform;
        manager_object = GameObject.Find("Game Manager");
        manager = manager_object.GetComponent<GameManager>();
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
        }
        else {
            nav.SetDestination(tauntVec);
        }
        if(HP <= 0 && deadFlag) {
            deadFlag = false;
            isLook = false;
            ani.SetTrigger("doDie");
            StopAllCoroutines();
            manager.GameClear();
            return;
        }
    }
    
    void LateUpdate() {
        
        if(deadFlag) {
            bossHPGroup.anchoredPosition = Vector3.down * 50;
            bossHPBar.localScale = new Vector3((float)HP / maxHP, 1, 1);
        }
        else {
            bossHPGroup.anchoredPosition = Vector3.up * 500;
        }
        
    }

    IEnumerator Think() {
        yield return new WaitForSeconds(0.1f);
        int rand = Random.Range(0, 7);
        switch(rand) {
            case 0:
            case 1:
            case 2:
                StartCoroutine("missileShoot");
                break;
            case 3:
            case 4:
            case 5:
                StartCoroutine("rockShoot");
                break;
            case 6:
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
        Instantiate(bullet_prefab, transform.position+transform.forward*5, transform.rotation);
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
        
        yield return new WaitForSeconds(1.5f);
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
