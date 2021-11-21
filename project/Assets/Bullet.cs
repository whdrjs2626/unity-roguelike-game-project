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

    void OnCollisionEnter(Collision other) {
        if((gameObject.name != "Boss Rock") && (other.gameObject.tag != "Floor") && (gameObject.GetComponent<Rigidbody>() != null)) Destroy(gameObject);
    }
    void Awake()
    {
        if(gameObject.name == "Missile Boss(Clone)") {
            nav = GetComponent<NavMeshAgent>();
        }
        if(gameObject.name == "Missile Boss(Clone)" || gameObject.name == "Missile(Clone)") Destroy(gameObject, 3);

        if(gameObject.name == "Bullet SubMachineGun(Clone)") {
            playerweapon = GameObject.Find("Weapon SubMachineGun").GetComponent<weapon>();
            //Destroy(gameObject, 1*playerweapon.range);
        }
    }

    void Update()
    {
        if(gameObject.name == "Missile Boss(Clone)") nav.SetDestination(target.position);
    }
}
