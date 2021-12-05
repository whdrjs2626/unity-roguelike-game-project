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

    protected bool deadFlag = false;

    void Awake() {
        deadFlag = true;
        player = GameObject.Find("Player").GetComponent<player>();
        target = GameObject.Find("Player").transform;
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mats = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>();
        if(etype != EnemyType.Boss) Invoke("ChaseStart", 2);
    }


    void OnTriggerEnter(Collider other) {
        
        if(other.tag == "Melee") {
            weapon weapon = other.GetComponent<weapon>();
            if(HP > 0) {
                HP -= weapon.damage;
                foreach(MeshRenderer mat in mats)
                    mat.material.color = Color.red;
                Invoke("OnDamage", 0.1f);
            }
            reactVec = transform.position - other.transform.position;
        }
        else if(other.name == "Bullet SubMachineGun(Clone)") {
            
            Bullet bullet = other.GetComponent<Bullet>();
            if(HP > 0) {
                HP -= bullet.damage;
                foreach(MeshRenderer mat in mats)
                    mat.material.color = Color.red;
                Invoke("OnDamage", 0.1f);
                Destroy(other.gameObject);
            }
            reactVec = transform.position - other.transform.position;
        }
    }

    void ChaseStart() {
        isChase = true;
        ani.SetBool("isWalk", true);
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position); 
        if((etype != EnemyType.Boss) && (distance <= 500.0f)) { // nav.enable
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        if(HP <= 0 && deadFlag) {
            foreach(MeshRenderer mat in mats)
                mat.material.color = Color.gray;
            
            gameObject.layer = 6;
            ani.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;
            rigid.AddForce((reactVec.normalized + Vector3.up) * 0.08f, ForceMode.Impulse);
            player player = target.GetComponent<player>();
            //Debug.Log("a");
            player.score += score;
            
            Invoke("itemDrop", 3);
            //if(etype != EnemyType.Boss) Destroy(gameObject, 4);
            Destroy(gameObject, 3);
            deadFlag = false;
        }
    }

    void itemDrop() {
        int rand = Random.Range(0, 20);
        switch(rand) {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                Instantiate(item_prefab[0], transform.position, Quaternion.identity);
                break;
            case 5:
            case 6:
            case 7:
                Instantiate(item_prefab[1], transform.position, Quaternion.identity);
                break;
            case 8:
            case 9:
                Instantiate(item_prefab[2], transform.position, Quaternion.identity);
                break;
            case 10:
            case 11:
                Instantiate(item_prefab[3], transform.position, Quaternion.identity);
                break;
            case 12:
            case 13:
                Instantiate(item_prefab[4], transform.position, Quaternion.identity);
                break;
            case 14:
                Instantiate(item_prefab[5], transform.position, Quaternion.identity);
                break;
            case 15:
                Instantiate(item_prefab[6], transform.position, Quaternion.identity);
                break;
            case 16:
                Instantiate(item_prefab[7], transform.position, Quaternion.identity);
                break;
            case 17:
                Instantiate(item_prefab[8], transform.position, Quaternion.identity);
                break;
            case 18:
                Instantiate(item_prefab[9], transform.position, Quaternion.identity);
                break;
            case 19:
                if(player.maxDashCount == 3) Instantiate(item_prefab[10], transform.position, Quaternion.identity);
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
            float targetRadius = 0f;
            float targetRange = 0f;

            switch(etype) {
                case EnemyType.A:
                    targetRadius = 1f;
                    targetRange = 3f;
                    break;
                case EnemyType.B:
                    targetRadius = 1f;
                    targetRange = 20f; // 크면 클수록 타겟팅을 멀리함
                    break;
                case EnemyType.C:
                    targetRadius = 0.5f;
                    targetRange = 80f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if(rayHits.Length > 0 && !isAttack) {
                StartCoroutine(Attack());
            } 
        }
    }

    IEnumerator Attack() {
        isChase = false;
        isAttack = true;
        ani.SetBool("isAttack", true);
        switch(etype) {
            case EnemyType.A:
                
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case EnemyType.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce((target.transform.position - transform.position) * 5, ForceMode.Impulse);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case EnemyType.C:
                yield return new WaitForSeconds(0.5f);
                GameObject bullet = Instantiate(bullet_prefab, transform.position, new Quaternion(transform.rotation.x, transform.rotation.y+bullet_prefab.transform.rotation.y, transform.rotation.z, transform.rotation.w));
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.Impulse);
                yield return new WaitForSeconds(2f);
                
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
