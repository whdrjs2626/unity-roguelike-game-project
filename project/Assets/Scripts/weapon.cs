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
    public int maxammo;
    public int ammo;
    public BoxCollider meleeArea;
    public TrailRenderer effect;
    public Transform bulletposition;
    public GameObject bullet_prefab;
    //public Transform bulletcaseposition;
    //public GameObject bulletcase_prefab; 

    //bool isShoot;

    public void Use(bool isShoot) {
        if(type == AttackType.Melee && !isShoot) {
            isShoot = true;
            StopCoroutine("Swing");
            StartCoroutine("Swing", isShoot);
        }
        else if(type == AttackType.Range && ammo > 0 && !isShoot) {
            isShoot = true;
            StartCoroutine("Shoot", isShoot); 
            ammo--;
        }
    }

    IEnumerator Swing(bool isShoot) {
        yield return new WaitForSeconds(0.1f);
        effect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        effect.enabled = false;       
        yield return new WaitForSeconds(rate);
        isShoot = false;
    }

    IEnumerator Shoot(bool isShoot) {
        GameObject bullet = Instantiate(bullet_prefab, bulletposition.position, bulletposition.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bulletposition.forward * 100, ForceMode.Impulse);
        yield return new WaitForSeconds(rate);
        isShoot = false;
    }
}
