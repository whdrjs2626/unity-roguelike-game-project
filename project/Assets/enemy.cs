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
    public GameObject[] coin_prefab;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] mats;
    public Vector3 reactVec;
    public NavMeshAgent nav;
    public Animator ani;

    protected virtual void Awake() {
        target = GameObject.Find("Player").transform;
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mats = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>();
        if(etype != EnemyType.Boss) Invoke("ChaseStart", 2);
    }


    void OnTriggerEnter(Collider other) {
        Debug.Log("a");
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
            Debug.Log("b");
            Bullet bullet = other.GetComponent<Bullet>();
            if(HP > 0) {
                HP -= bullet.damage;
                foreach(MeshRenderer mat in mats)
                    mat.material.color = Color.red;
                Invoke("OnDamage", 0.1f);
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
        if(nav.enabled && (etype != EnemyType.Boss)) {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        if(HP <= 0) {
            foreach(MeshRenderer mat in mats)
                mat.material.color = Color.gray;
            gameObject.layer = 6;
            ani.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;
            rigid.AddForce((reactVec.normalized + Vector3.up) * 0.08f, ForceMode.Impulse);
            player player = target.GetComponent<player>();
            player.score += score;
            int ranCoin = Random.Range(0, 3);
            Instantiate(coin_prefab[ranCoin], transform.position, Quaternion.identity);
            //if(etype != EnemyType.Boss) Destroy(gameObject, 4);
            Destroy(gameObject, 4);
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
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case EnemyType.B:
                    targetRadius = 1f;
                    targetRange = 6f; // 크면 클수록 타겟팅을 멀리함
                    break;
                case EnemyType.C:
                    targetRadius = 0.5f;
                    targetRange = 20f;
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
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case EnemyType.C:
                yield return new WaitForSeconds(0.5f);
                GameObject bullet = Instantiate(bullet_prefab, transform.position, new Quaternion(transform.rotation.x, transform.rotation.y+90, transform.rotation.z, transform.rotation.w));
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
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
