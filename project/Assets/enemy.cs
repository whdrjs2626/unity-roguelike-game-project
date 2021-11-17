using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C };
    public EnemyType etype;
    public int maxHP;
    public int HP;
    public bool isChase, isAttack;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet_prefab;
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    Vector3 reactVec;
    NavMeshAgent nav;
    Animator ani;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>();
        Invoke("ChaseStart", 2);
    }


    void OnTriggerEnter(Collider other) {
        if(other.tag == "Melee") {
            weapon weapon = other.GetComponent<weapon>();
            if(HP > 0) {
                HP -= weapon.damage;
                Debug.Log("melle : " + HP);
                mat.color = Color.red;
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
        if(nav.enabled) {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        if(HP <= 0) {
            mat.color = Color.gray;
            gameObject.layer = 6;
            ani.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;
            rigid.AddForce((reactVec.normalized + Vector3.up) * 0.08f, ForceMode.Impulse);
            Destroy(gameObject, 4);
        }
    }

    void OnDamage() {
        if(HP > 0) mat.color = Color.white;
    }

    void Targeting() {
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
                GameObject bullet = Instantiate(bullet_prefab, transform.position, transform.rotation);
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
