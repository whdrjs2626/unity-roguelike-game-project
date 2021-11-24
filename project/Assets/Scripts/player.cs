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

    public int dashCount = 3, maxDashCount = 3;
    public GameObject[] Weapon;
    public bool[] hasWeapon;
    
    public int curAmmo, Ammo, maxAmmo;
    public int coin;
    public int health;

    public int maxhealth;
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
        StartCoroutine("DashCountUp");
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
        if(dash && !isDash && !isDead && moveVec != Vector3.zero && dashCount > 0) {
            isDash = true;
            speed *= 2.0f;
            ani.SetTrigger("doDash");
            dashCount--;
            Invoke("DashEnd", 2);
        }
        if(dashCount == 1) {
            manager.Dashs[0].SetActive(true);
            manager.Dashs[1].SetActive(false);
            manager.Dashs[2].SetActive(false);
            manager.Dashs[3].SetActive(false);
        }
        else if(dashCount == 2) {
            manager.Dashs[0].SetActive(true);
            manager.Dashs[1].SetActive(true);
            manager.Dashs[2].SetActive(false);
            manager.Dashs[3].SetActive(false);
        }
        else if(dashCount == 3) {
            manager.Dashs[0].SetActive(true);
            manager.Dashs[1].SetActive(true);
            manager.Dashs[2].SetActive(true);
            manager.Dashs[3].SetActive(false);
        }
        else if(dashCount == 4) {
            manager.Dashs[0].SetActive(true);
            manager.Dashs[1].SetActive(true);
            manager.Dashs[2].SetActive(true);
            manager.Dashs[3].SetActive(true);
        }
        else if(dashCount == 0) {
            manager.Dashs[0].SetActive(false);
            manager.Dashs[1].SetActive(false);
            manager.Dashs[2].SetActive(false);
            manager.Dashs[3].SetActive(false);
        }
    }
    IEnumerator DashCountUp() {
        while(true) {
            yield return new WaitForSeconds(6f);
            if(dashCount < maxDashCount) {
                dashCount++;
            }
        }
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
            bool isShoot = false;
            attackdelay += Time.deltaTime;
            attackready = myweapon.rate < attackdelay;

            if(attack && attackready && !isDash && curAmmo > 0) {
                myweapon.Use(isShoot);
                if(myweapon.type == weapon.AttackType.Melee && !isShoot) ani.SetTrigger("doAttack");
                else if(myweapon.type == weapon.AttackType.Range && !isShoot) {
                    ani.SetTrigger("doShoot");
                    curAmmo--;
                }
                attackdelay = 0;
            }
        }
    }
    
    void Reload() {
        if((myweapon.gameObject == null) || (myweapon.type == weapon.AttackType.Melee) || maxAmmo <= 0) return;
        else if(reload && !isreload && !attack && !dash) {
            isreload = true;
            ani.SetTrigger("doReload");
            Invoke("ReloadEnd", 2f);
        }
    }

    void ReloadEnd() {      
        int needAmmo = Ammo - curAmmo; // 현재 탄창에 필요한 탄약 수
        if(maxAmmo > needAmmo) { // 장전해야 할 탄약 수보다 가지고 있는 탄약 수가 많은 경우
            maxAmmo -= needAmmo;
            curAmmo = Ammo; 
        }
        else { // 현재 가진 탄약 총 개수가 탄창 용량보다 적을 시
            curAmmo += maxAmmo;
            maxAmmo = 0;
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
        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
        isFloor = Physics.Raycast(transform.position, forwardVec, 2, LayerMask.GetMask("Wall"));
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
                    maxAmmo += item.value;
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
                        myweapon.bullet_prefab.GetComponent<Bullet>().damage += 5;
                    }
                    break;
                case Item.ItemType.SPDUP:
                    speed += 10;
                    break;
                case Item.ItemType.RANGEUP:
                    if(hammer) {
                        myweapon.transform.localScale += new Vector3(0.5f, 0.8f, 0.5f);
                    }
                    else if(gun) {
                        myweapon.range += 0.5f;
                    }
                    break;
                case Item.ItemType.RATEUP:
                    myweapon.rate -= 0.1f;
                    break;
                case Item.ItemType.MAXHPUP:
                    maxhealth += 50;
                    break;
                case Item.ItemType.DASHUP:
                    manager.plusDash.SetActive(true);
                    maxDashCount++;
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

