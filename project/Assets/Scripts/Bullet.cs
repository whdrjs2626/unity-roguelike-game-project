using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    public int damage;
    public Transform target;
    NavMeshAgent nav;
    weapon playerweapon;

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Wall") Destroy(gameObject);
        if((this.name == "Boss Melee Area") && (other.gameObject.tag == "Player")) {
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 500, ForceMode.Impulse);
        }
    }
    void Awake()
    {
        if(gameObject.name == "Missile Boss(Clone)") {
            nav = GetComponent<NavMeshAgent>();
        }
        if(gameObject.name == "Missile Boss(Clone)" || gameObject.name == "Missile(Clone)") Destroy(gameObject, 5);

        if(gameObject.name == "Bullet SubMachineGun(Clone)") {
            playerweapon = GameObject.Find("Weapon SubMachineGun").GetComponent<weapon>();
            Destroy(gameObject, 1*playerweapon.range);
        }
    }

    void Update()
    {
        if(gameObject.name == "Missile Boss(Clone)") nav.SetDestination(target.position);
    }
}
