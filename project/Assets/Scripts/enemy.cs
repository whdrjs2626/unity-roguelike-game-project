using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C, Boss };
    public EnemyType etype;
    public int maxHP;
    public int HP;
    public int score;
    public bool isChase, isAttack;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet_prefab;
    public GameObject[] item_prefab;
    public player player;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] mats;
    public Vector3 reactVec;
    public NavMeshAgent nav;
    public Animator ani;
    public GameManager manager;

    public AudioClip attackSound;
    public AudioClip swordattackedSound;
    public AudioClip gunattackedSound;
    public AudioClip dieSound;
    protected bool deadFlag = false;

    public GameObject attackEffect;

    float distance;

    void Awake() {
        deadFlag = true;
        player = GameObject.Find("Player").GetComponent<player>();
        target = GameObject.Find("Player").transform;
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mats = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>();
        if(etype != EnemyType.Boss) Invoke("ChaseStart", 2);
    }


    void OnTriggerEnter(Collider other) {
        if(other.tag == "Melee") { // 근접 공격의 경우
            AudioSource.PlayClipAtPoint(swordattackedSound, this.transform.position);
            weapon weapon = other.GetComponent<weapon>();
            if(HP > 0) { // 피격은 살아있을 때만 가능
                HP -= weapon.damage; // 체력 감소
                foreach(MeshRenderer mat in mats) // 몬스터의 모든 Mesh를 빨간색으로 변경(피격효과)
                    mat.material.color = Color.red;
                Invoke("OnDamage", 0.1f); // 0.1초 뒤 OnDamage함수 호출
            }
        }
        else if(other.tag == "Range") { // 원거리 공격
            AudioSource.PlayClipAtPoint(gunattackedSound, this.transform.position);
            Bullet bullet = other.GetComponent<Bullet>();
            if(HP > 0) {
                HP -= bullet.damage; // 체력 감소
                foreach(MeshRenderer mat in mats) // 피격효과 (빨간색)
                    mat.material.color = Color.red;
                Invoke("OnDamage", 0.1f); // 0.1초 뒤 OnDamage함수 호출
                Destroy(other.gameObject); // 총알 없앰
            }
        }
    }

    void ChaseStart() {
        isChase = true;
        ani.SetBool("isWalk", true);
    }

    void Update()
    {
        distance = Vector3.Distance(target.position, transform.position); 
        if((etype != EnemyType.Boss) && (distance <= 500.0f)) { 
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        if(HP <= 0 && deadFlag) { // deadFlag는 아래 명령어를 한번만 실행할 수 있게 하기 위함
            foreach(MeshRenderer mat in mats)
                mat.material.color = Color.gray; // 죽으면 몬스터 색상을 회색으로 변경
            AudioSource.PlayClipAtPoint(dieSound, this.transform.position); // 죽는 사운드 재생
            gameObject.layer = 6; // 몬스터의 layer을 DeadEnemy로 변경
            if(etype != EnemyType.C) Destroy(meleeArea); // 죽고 나서 공격하는 것을 막음
            ani.SetTrigger("doDie"); // 죽음 모션 수행 
            isChase = false; // 플레이어를 추적하지 않음
            nav.enabled = false; 
            rigid.AddForce(((transform.position - target.transform.position).normalized + Vector3.up) * 0.08f, ForceMode.Impulse);
            // 죽으면서 살짝 뒤로 밀려나게 함
            player player = target.GetComponent<player>();
            player.score += score; // 플레이어에게 점수를 줌
            Invoke("itemDrop", 3); // 3초 뒤 아이템 드롭
            Destroy(gameObject, 3); // 3초 뒤 몬스터 오브젝트 삭제
            deadFlag = false; 
        }
    }

    void itemDrop() {
        int rand = Random.Range(0, 30);
        switch(rand) {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                break; // 노드롭
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                Instantiate(item_prefab[0], transform.position, Quaternion.identity); // 코인(동)
                break;
            case 15:
            case 16:
            case 17:
                Instantiate(item_prefab[1], transform.position, Quaternion.identity); // 코인(은)
                break;
            case 18:
            case 19:
                Instantiate(item_prefab[2], transform.position, Quaternion.identity); // 코인(금)
                break;
            case 20:
            case 21:
                Instantiate(item_prefab[3], transform.position, Quaternion.identity); // 탄약
                break;
            case 22:
            case 23:
                Instantiate(item_prefab[4], transform.position, Quaternion.identity); // 체력
                break;
            case 24:
                Instantiate(item_prefab[5], transform.position, Quaternion.identity); // 공격력 증가
                break;
            case 25:
                Instantiate(item_prefab[6], transform.position, Quaternion.identity); // 최대 체력 증가
                break;
            case 26:
                Instantiate(item_prefab[7], transform.position, Quaternion.identity); // 사거리 증가
                break;
            case 27:
                Instantiate(item_prefab[8], transform.position, Quaternion.identity); // 공격 속도 증가
                break;
            case 28:
                Instantiate(item_prefab[9], transform.position, Quaternion.identity); // 이동 속도 증가
                break;
            case 29:
                if(player.maxDashCount == 3) Instantiate(item_prefab[10], transform.position, Quaternion.identity); // 대시 회수 증가
                break;
        }
    }

    void OnDamage() {
        if(HP > 0) {
            foreach(MeshRenderer mat in mats)
            mat.material.color = Color.white;
        }
    }

    void Targeting() {
        if(etype != EnemyType.Boss && HP > 0) {
            transform.LookAt(target);
            switch(etype) {
                case EnemyType.A:
                    if(distance < 4f && !isAttack) StartCoroutine(Attack());
                    break;
                case EnemyType.B:
                    if(distance < 12f && !isAttack) StartCoroutine(Attack());
                    break;
                case EnemyType.C:
                    if(distance < 50f && !isAttack) StartCoroutine(Attack());
                    break;
            }
            /*
            float radius = 0f;
            float maxDistance = 0f;
            switch(etype) {
                case EnemyType.A:
                    radius = 1f;
                    maxDistance = 3f;
                    break;
                case EnemyType.B:
                    radius = 1f;
                    maxDistance = 20f; // 크면 클수록 타겟팅을 멀리함 - 돌진
                    break;
                case EnemyType.C:
                    radius = 0.5f;
                    maxDistance = 80f;
                    break;
            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, radius, transform.forward, maxDistance, LayerMask.GetMask("Player"));
            if(rayHits.Length > 0 && !isAttack) {
                StartCoroutine(Attack());
            } 
            */
        }
    }

    IEnumerator Attack() {
        isChase = false;
        isAttack = true;
        ani.SetBool("isAttack", true);
        switch(etype) {
            case EnemyType.A:
                yield return new WaitForSeconds(0.1f);
                AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
                meleeArea.enabled = true;  // 공격을 위한 Object 활성화
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false; // 공격을 위한 Object 비활성화
                yield return new WaitForSeconds(1f);
                break;
            case EnemyType.B:
                attackEffect.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                //(target.transform.position - transform.position) 
                rigid.AddForce(transform.forward* 25, ForceMode.Impulse);
                AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
                meleeArea.enabled = true;  // 공격을 위한 Object 활성화
                yield return new WaitForSeconds(0.3f);
                attackEffect.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false; // 공격을 위한 Object 비활성화
                yield return new WaitForSeconds(4f);
                break;
            case EnemyType.C:
                yield return new WaitForSeconds(0.5f);
                attackEffect.SetActive(true);
                AudioSource.PlayClipAtPoint(attackSound, this.transform.position);
                GameObject bullet = Instantiate(bullet_prefab, transform.position, new Quaternion(transform.rotation.x, transform.rotation.y+bullet_prefab.transform.rotation.y, transform.rotation.z, transform.rotation.w));
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.Impulse); // 미사일 발사
                yield return new WaitForSeconds(0.3f);
                attackEffect.SetActive(false);
                yield return new WaitForSeconds(1.7f);
                break;
        }
        isChase = true;
        isAttack = false;
        ani.SetBool("isAttack", false);
    }

    void FixedUpdate() {
        Targeting();
    }

}
