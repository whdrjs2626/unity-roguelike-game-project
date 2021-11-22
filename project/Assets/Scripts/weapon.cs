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

    public void Use() {
        Debug.Log("attack");
        if(type == AttackType.Melee) {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(type == AttackType.Range && ammo > 0) {
            Shoot(); 
            ammo--;
        }
    }

    IEnumerator Swing() {
        yield return new WaitForSeconds(0.1f);
        
        effect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        effect.enabled = false;       
    }

    void Shoot() {
        GameObject bullet = Instantiate(bullet_prefab, bulletposition.position, bulletposition.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bulletposition.forward * 100, ForceMode.Impulse);
        //yield return new WaitForSeconds(0.1f);
    }
}
