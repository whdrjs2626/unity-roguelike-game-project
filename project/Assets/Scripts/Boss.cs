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

    public AudioClip rockSound;
    public AudioClip tauntSound;
    public AudioClip startSound;

    


    void Awake() {
        AudioSource.PlayClipAtPoint(startSound, this.transform.position);
        deadFlag = true;
        temp = GameObject.Find("Boss Group");
        bossHPGroup = GameObject.Find("Boss Group").GetComponent<RectTransform>();
        temp = GameObject.Find("Boss Real HP");
        bossHPBar = GameObject.Find("Boss Real HP").GetComponent<RectTransform>();
        target = GameObject.Find("Player").transform;
        manager_object = GameObject.Find("Game Manager");
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mats = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>();

        
        isLook = true;
        nav.isStopped = true;
        StartCoroutine("idle");
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
            deadFlag = false; // 죽음 명령어를 한번만 수행하기 위한 플래그
            isLook = false; // 플레이어를 쳐다보지 않음
            ani.SetTrigger("doDie"); // 죽음 모션
            StopAllCoroutines(); // 모든 코루틴 멈춤
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                enemy.GetComponent<enemy>().HP = 0; // 필드의 모든 적을 다 죽입니다.   
            manager.GameClear(); // GameClear() 함수 호출
            return;
        }
    }
    
    void LateUpdate() {
        if(deadFlag) {
            bossHPGroup.anchoredPosition = new Vector3(3, -1, 0) * 50;
            bossHPBar.localScale = new Vector3((float)HP / maxHP, 1, 1);
        }
        else {
            bossHPGroup.anchoredPosition = Vector3.up * 500;
        }
        
    }

    IEnumerator idle() {
        yield return new WaitForSeconds(0.1f);
        int rand = Random.Range(0, 10);
        switch(rand) {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                StartCoroutine("missileShoot");
                break;
            case 6:
            case 7:
            case 8:
                StartCoroutine("rockShoot");
                break;
            case 9:
                StartCoroutine("Taunt");
                break;
        }
    }

    IEnumerator missileShoot() {
        ani.SetTrigger("doShot"); // 미사일 발사 모션 
        yield return new WaitForSeconds(0.2f); // 미사일 A 발사
        AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
        GameObject missileA = Instantiate(missile_prefab, missilePortA.position, missilePortA.rotation);
        Bullet bossMissileA = missileA.GetComponent<Bullet>();
        bossMissileA.target = target;
        yield return new WaitForSeconds(0.3f); // 미사일 B 발사
        AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
        GameObject missileB = Instantiate(missile_prefab, missilePortB.position, missilePortB.rotation);
        Bullet bossMissileB = missileB.GetComponent<Bullet>();
        bossMissileB.target = target;
        yield return new WaitForSeconds(2.5f);
        StartCoroutine("idle"); // 다시 idle로 
    }

    IEnumerator rockShoot() {
        AudioSource.PlayClipAtPoint(rockSound, this.transform.position);
        isLook = false;
        ani.SetTrigger("doRock");
        Instantiate(bullet_prefab, transform.position+transform.forward*5, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine("idle");
    }

    IEnumerator Taunt() {
        isLook = false;
        nav.isStopped = false;
        tauntVec = target.position + lookVec;
        boxCollider.enabled = false; // 점프 뛸 때 플레이어를 밀지 않도록
        ani.SetTrigger("doTaunt");
        
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        AudioSource.PlayClipAtPoint(tauntSound, this.transform.position);
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        boxCollider.enabled = true;
        nav.isStopped = true;
        isLook = true;
        StartCoroutine("idle");
    }
}
