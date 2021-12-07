using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public enum AttackType { Melee, Range };
    public AttackType type;
    public int damage;
    public float rate;
    public float range;
    public BoxCollider meleeArea;
    public TrailRenderer effect;
    public Transform bulletposition;
    public GameObject bullet_prefab;
    public player player;

    //public Transform bulletcaseposition;
    //public GameObject bulletcase_prefab; 

    //bool isShoot;

    public void Use() {
        if(type == AttackType.Melee && !player.isShoot) {
            player.isShoot = true;
            StartCoroutine("Swing");
        }
        else if(type == AttackType.Range && !player.isShoot) {
            player.isShoot = true;
            StartCoroutine("Shoot"); 
        }
    }

    IEnumerator Swing() {
        yield return new WaitForSeconds(0.1f);
        effect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.1f);
        effect.enabled = false;       
        yield return new WaitForSeconds(rate*2);
        player.isShoot = false;
    }

    IEnumerator Shoot() {
        GameObject bullet = Instantiate(bullet_prefab, bulletposition.position, bulletposition.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bulletposition.forward * 100, ForceMode.Impulse);
        yield return new WaitForSeconds(rate);
        player.isShoot = false;
    }
}
