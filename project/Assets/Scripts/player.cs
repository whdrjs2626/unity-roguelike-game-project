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
    public bool hammer, gun;
    bool attack, attackready;
    bool reload, isreload;
    bool isDamaged;
    bool isBorder;
    bool isFloor;
    bool isShop; // 쇼핑중인가
    bool isDead;
    float attackdelay;

    public Camera followCamera;

    int dashcount = 3, maxdashcount = 3;
    public GameObject[] Weapon;
    public bool[] hasWeapon;
    
    public int ammo;
    public int coin;
    public int health;

    public int maxhealth;
    public int maxammo;
    public int score;

    Animator ani;
    Vector3 moveVec, forwardVec;
    Rigidbody rigid;
    MeshRenderer[] mat;
    GameObject Object;
    public weapon myweapon;
    public GameManager manager;
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
        if(!isBorder && !isFloor && !isDead) {
            if(!walk) transform.position += moveVec * speed * Time.deltaTime; // 방향키를 누른 방향으로 이동
            else transform.position += moveVec * speed/2 * Time.deltaTime;
        }
        ani.SetBool("isRun", moveVec != Vector3.zero);
        ani.SetBool("isWalk", walk);
    }
    void Turn() {
        if(!isDead) {
            transform.LookAt(transform.position + moveVec);  
        //if(attack) {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)) {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                forwardVec = transform.position + nextVec;
                transform.LookAt(transform.position + nextVec);
            } 
        //}
        }
    }
    void Dash() {
        if(dash && !isDash && !isDead && moveVec != Vector3.zero) {
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
                gun = false;
            }
            if((Object.tag == "Weapon") && (Object.name == "Weapon SubMachineGun")) {
                Item item = Object.GetComponent<Item>();
                int i = item.value;
                hasWeapon[i] = true;
                Destroy(Object);
                Destroy(GameObject.Find("Weapon Hammer"));
                gun = true;
                hammer = false;
            }
            if(Object.tag == "Shop") { // 상호작용을 하는 오브젝트의 태그가 상점인 경우
                Shop shop = Object.GetComponent<Shop>();
                shop.Enter(this); // Shop의 Enter 함수 호출
                isShop = true;
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
        if(myweapon.gameObject == null || isShop || isDead) { // 무기가 없거나 상점에서 쇼핑중일 땐 공격 불가능
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

    void FreezeRotation() {
        rigid.angularVelocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
    }
    void StopToWall() {
        //isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //Debug.DrawRay(transform.position, forwardVec * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 10, LayerMask.GetMask("Wall"));
        isFloor = Physics.Raycast(transform.position, forwardVec, 10, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate() {
        FreezeRotation();
        StopToWall();
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
                    //If((health > maxhealth)) { health = maxhealth;}  
                    break;
                case Item.ItemType.ATKUP:
                    if(hammer) {
                        myweapon.damage += 10;
                    }
                    else if(gun) {
                        //myweapon.
                    }
                    break;
                case Item.ItemType.SPDUP:
                    speed += 10;
                    break;
                case Item.ItemType.RANGEUP:
                    break;
                case Item.ItemType.RATEUP:
                    break;
                case Item.ItemType.MAXHPUP:
                    maxhealth += 50;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "Enemy") {

        }
        else if(other.tag == "EnemyBullet") { 
            if(!isDamaged) { 
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                bool isBossAtk = (other.name == "Boss Melee Area");
                //if(other.GetComponent<Rigidbody>() != null) {
                //    Destroy(other.gameObject);
                //}
                StartCoroutine("OnDamage", isBossAtk);
            }
            if(other.GetComponent<Rigidbody>() != null) {
                Destroy(other.gameObject); // 무적과 상관 없이 투사체는 무조건 없어짐
            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk) {
        isDamaged = true;
        foreach(MeshRenderer m in mat) {
            m.material.color = Color.yellow;
        }
        if(isBossAtk) rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        if(health <= 0 && !isDead) {
            OnDie();
        }   
        yield return new WaitForSeconds(1f);
        isDamaged = false;
        foreach(MeshRenderer m in mat) {
            m.material.color = Color.white;
        }
        if(isBossAtk) rigid.velocity = Vector3.zero;     
        
        
    }

    void OnDie() {
        ani.SetTrigger("doDie");
        GetComponent<Rigidbody>().isKinematic = true;
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerStay(Collider other) {
        if(other.tag == "Weapon" || other.tag == "Shop") Object = other.gameObject;
    }

    void OnTriggerExit(Collider other) {
        //if(other.tag == "Shop") Object = null;
        if(other.tag == "Shop") {
            Shop shop = Object.GetComponent<Shop>();
            shop.Exit();
            Object = null;
            isShop = false;
        }
    }   
}

