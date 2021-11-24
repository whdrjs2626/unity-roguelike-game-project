using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Ammo, Coin, Heart, Weapon, ATKUP, SPDUP, RANGEUP, RATEUP, MAXHPUP, DASHUP };
    public ItemType type;
    public int value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
