using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed, timer;
    float h, v;
    bool walk;
    bool dash, isDash;
    bool iDown;
    bool hammer, gun;
    bool attack, attackready;
    bool reload, isreload;
    bool isDamaged;
    //bool isBorder;
    float attackdelay;

    public Camera followCamera;

    int dashcount = 3, maxdashcount = 3;
    public GameObject[] Weapon;
    public bool[] hasWeapon;
    
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenade;

    public int maxhealth;
    public int maxammo;

    Animator ani;
    Vector3 moveVec;
    Rigidbody rigid;
    MeshRenderer[] mat;
    GameObject Object;
    weapon myweapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake() 
    {
        rigid = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        mat = GetComponentsInChildren<MeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        GetInput();
        Move();
        Turn();
        Dash();
        InterAction();
        Equip();
        Attack();
        Reload();
    }

    void GetInput() {        
        h = Input.GetAxisRaw("Horizontal"); // left, right button
        v = Input.GetAxisRaw("Vertical"); // up, down button
        walk = Input.GetButton("Walk"); // shift button
        dash = Input.GetButtonDown("Jump"); // space button
        iDown = Input.GetButtonDown("Interaction"); // E
        attack = Input.GetButton("Fire1");
        reload = Input.GetButtonDown("Reload");
    }
    void Move() {
        moveVec = new Vector3(h, 0, v).normalized; // normalized를 통해 모든 경우 1로 정규화
        if(!walk) transform.position += moveVec * speed * Time.deltaTime; // 방향키를 누른 방향으로 이동
        else transform.position += moveVec * speed/2 * Time.deltaTime;
        ani.SetBool("isRun", moveVec != Vector3.zero);
        ani.SetBool("isWalk", walk);
    }
    void Turn() {
        transform.LookAt(transform.position + moveVec);  
        //if(attack) {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)) {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            } 
        //}
    }
    void Dash() {
        if(dash && !isDash && moveVec != Vector3.zero && dashcount > 0) {
            isDash = true;
            speed *= 2.0f;
            ani.SetTrigger("doDash");
            dashcount--;
            Invoke("DashEnd", 2);
        }
    }
    void DashCountUp() {
        dashcount++;
    }
    void DashEnd() {
        speed *= 0.5f;
        isDash = false;
    }

    void InterAction() {
        if(iDown && Object != null) { // e가 눌리면서 근처에 오브젝트가 있으면 상호작용
            if((Object.tag == "Weapon") && (Object.name == "Weapon Hammer")) {
                
                Item item = Object.GetComponent<Item>();
                int i = item.value;
                hasWeapon[i] = true;
                Destroy(Object);
                Destroy(GameObject.Find("Weapon SubMachineGun"));
                hammer = true;
                //Weapon = 
            }
            if((Object.tag == "Weapon") && (Object.name == "Weapon SubMachineGun")) {
                Item item = Object.GetComponent<Item>();
                int i = item.value;
                hasWeapon[i] = true;
                Destroy(Object);
                Destroy(GameObject.Find("Weapon Hammer"));
                gun = true;
                //Weapon = 
            }
        }
    }
    void Equip() {
        if(hammer) {
            myweapon = Weapon[0].GetComponent<weapon>();
            Weapon[0].SetActive(true);
            Weapon[1].SetActive(false);
        }
        if(gun) {
            myweapon = Weapon[1].GetComponent<weapon>();
            Weapon[1].SetActive(true);
            Weapon[0].SetActive(false);
        }
    }

    void Attack() {
        if(myweapon.gameObject == null) {
            return;
        }
        else {
            attackdelay += Time.deltaTime;
            attackready = myweapon.rate < attackdelay;

            if(attack && attackready && !isDash) {
                myweapon.Use();
                if(myweapon.type == weapon.AttackType.Melee) ani.SetTrigger("doAttack");
                else if(myweapon.type == weapon.AttackType.Range) ani.SetTrigger("doShoot");
                attackdelay = 0;
            }
        }
    }
    
    void Reload() {
        if((myweapon.gameObject == null) || (myweapon.type == weapon.AttackType.Melee) || ammo <= 0) return;
        else if(reload && !isreload && !attack && !dash) {
            isreload = true;
            ani.SetTrigger("doReload");
            Invoke("ReloadEnd", 2f);
        }
    }

    void ReloadEnd() {
        if(ammo < myweapon.maxammo) {
            myweapon.ammo = ammo;
            ammo -= ammo;
        }
        else {
            myweapon.ammo = myweapon.maxammo;
            ammo -= myweapon.maxammo;
        }
        isreload = false;
    }

    void FreezeRatation() {
        rigid.angularVelocity = Vector3.zero;
    }
    //void StopToWall() {
    //    isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    //}
    void FixedUpdate() {
        FreezeRatation();
    //    StopToWall();
    }
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch(item.type) {
                case Item.ItemType.Ammo:
                    ammo += item.value;
                   // If(ammo > maxammo) { ammo = maxammo; }
                    break;
                case Item.ItemType.Coin:
                    coin += item.value;
                    break;
                case Item.ItemType.Heart:
                    health += item.value;
                   // If(health > maxhealth) { health = maxhealth; }
                    break;
                case Item.ItemType.Grenade:
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "Enemy") {

        }
        else if(other.tag == "EnemyBullet") { 
            Debug.Log("out");
            if(!isDamaged) { 
                Debug.Log("in");
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                if(other.GetComponent<Rigidbody>() != null) {
                    Destroy(other.gameObject);
                }
                StartCoroutine("OnDamage");
            }
        }
    }
    IEnumerator OnDamage() {
        isDamaged = true;
        foreach(MeshRenderer m in mat) {
            m.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1f);
        isDamaged = false;
        foreach(MeshRenderer m in mat) {
            m.material.color = Color.white;
        }
    }
    void OnTriggerStay(Collider other) {
        if(other.tag == "Weapon") Object = other.gameObject;
    }


    
}

