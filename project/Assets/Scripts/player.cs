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
    public bool sword, gun;
    bool attack;
    bool reload, isreload;
    bool isDamaged;
    bool isWall;
    bool isShop; // 쇼핑중인가
    bool isDead;
    public Camera followCamera;

    public int dashCount = 3, maxDashCount = 3;
    public GameObject[] Weapon;
    public bool[] hasWeapon;

    public bool isShoot = false; // 공격 딜레이를 위한 변수 - 공격중인가? - false일 때 공격 가능
         
    public int curAmmo, Ammo, maxAmmo;
    public int coin;
    public int health;

    public int maxhealth;
    public int score;

    public AudioClip shootSound;
    public AudioClip swingSound;
    public AudioClip attackedSound;
    public AudioClip reloadSound;
    public AudioClip coinSound;
    public AudioClip statitemSound;
    public AudioClip dashSound;

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
        if(!isWall && !isDead) {
            if(!walk) transform.position += moveVec * speed * Time.deltaTime; // 방향키를 누른 방향으로 이동
            else transform.position += moveVec * speed/2 * Time.deltaTime;
        }
        ani.SetBool("isRun", moveVec != Vector3.zero);
        ani.SetBool("isWalk", walk);
    }
    void Turn() {
        if(!isDead) {
            transform.LookAt(transform.position + moveVec);
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)) {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                forwardVec = transform.position + nextVec;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }
    void Dash() {
        if(dash && !isDash && !isDead && moveVec != Vector3.zero && dashCount > 0) {
            // 스페이스바를 누름 && 대시중X && 살아있음 && 이동 중일때만 && 대시 카운트가 1 이상일 때 대시 가능
            AudioSource.PlayClipAtPoint(dashSound, this.transform.position); // 대시 사운드
            isDash = true; // 대시를 시작했으므로 대시 중임을 알림
            speed *= 1.5f; // 이동속도를 1.5배로 변경
            ani.SetTrigger("doDash"); // 대시 모션 수행
            dashCount--; // 현재 대시 횟수 1 감소
            Invoke("DashEnd", 2); // 2초 뒤 대시 효과를 없애는 함수 호출
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

    void DashEnd() {
        speed /= 3f;
        speed *= 2f;
        isDash = false;
    }

    IEnumerator DashCountUp() {
        while(true) {
            yield return new WaitForSeconds(5f);
            if(dashCount < maxDashCount) {
                dashCount++;
            }
        }
    }
    

    void InterAction() {
        if(iDown && Object != null) { // e가 눌리면서 근처에 오브젝트가 있으면 상호작용
            if((Object.tag == "Weapon") && (Object.name == "Weapon Sword")) {
                Item item = Object.GetComponent<Item>();
                int i = item.value; // 검의 value = 0
                hasWeapon[i] = true; // 0번째 무기(검) = true
                Destroy(Object); // 검 아이템 삭제
                Destroy(GameObject.Find("Weapon SubMachineGun")); // 총 아이템 삭제
                sword = true; // 검 변수 true
                gun = false; // 총 변수 false
            }
            if((Object.tag == "Weapon") && (Object.name == "Weapon SubMachineGun")) {
                Item item = Object.GetComponent<Item>(); 
                int i = item.value; // 총의 value = 1
                hasWeapon[i] = true; // 배열의 1번째 무기(총) = true 
                Destroy(Object); // 총 아이템 삭제
                Destroy(GameObject.Find("Weapon Sword")); // 검 아이템 삭제
                gun = true; // 총 변수 true
                sword = false; // 검 변수 false
            }
            if(Object.tag == "Shop") { // 상호작용을 하는 오브젝트의 태그가 상점인 경우
                Shop shop = Object.GetComponent<Shop>();
                shop.Enter(this); // Shop의 Enter 함수 호출
                isShop = true;
            }
        }
    }
    void Equip() {
        if(sword) {
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
        if(!(sword || gun) || isShop || isDead) { // 무기가 없거나 상점에서 쇼핑중일 땐 공격 불가능
            return;
        }
        else if(attack && !isDash && curAmmo > 0 && !isShoot) { // 좌클릭 && 대쉬중이지 않음 && 탄약 1 이상 && 공격중이지 않음
                if(myweapon.type == weapon.AttackType.Melee) { // 근접 공격인 경우(검)
                    myweapon.Use(); // 공격 함수
                    AudioSource.PlayClipAtPoint(swingSound, this.transform.position); // 검 휘두르는 사운드
                    ani.SetTrigger("doAttack"); // 검 휘두르는 모션
                }
                else if(myweapon.type == weapon.AttackType.Range) { // 원거리 공격인 경우(총)
                    myweapon.Use(); // 공격 함수
                    AudioSource.PlayClipAtPoint(shootSound, this.transform.position); // 총 발사 사운드
                    ani.SetTrigger("doShoot"); // 총 쏘는 모션
                    curAmmo--; // 탄약 1 감소
                }
        }
    }
    
    void Reload() {
        if(!gun || maxAmmo <= 0) return;
        // 무기가 총이 아니거나 보유 탄약이 없는 경우 장전 불가능
        else if(reload && !isreload && !attack && !dash) { // 위 경우를 제외하고 R을 누른 상태에서 공격, 장전, 대시 중이 아닌 경우
            AudioSource.PlayClipAtPoint(reloadSound, this.transform.position); // 재장전 사운드
            isreload = true; // 장전중인가? true
            ani.SetTrigger("doReload"); // 장전 모션 수행
            Invoke("ReloadEnd", 2f); // 2초 뒤 장전 종료 함수 수행
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
    void stopMove() {
        isWall = Physics.Raycast(transform.position, moveVec, 5, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate() {
        FreezeRotation();
        stopMove();
    }
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch(item.type) {
                case Item.ItemType.Ammo:
                    AudioSource.PlayClipAtPoint(coinSound, this.transform.position);
                    maxAmmo += item.value;
                    break;
                case Item.ItemType.Coin:
                    AudioSource.PlayClipAtPoint(coinSound, this.transform.position);
                    coin += item.value;
                    break;
                case Item.ItemType.Heart:
                    AudioSource.PlayClipAtPoint(coinSound, this.transform.position);
                    if((health+item.value) > maxhealth) health = maxhealth;
                    else health += item.value; 
                    break;
                case Item.ItemType.ATKUP:
                    AudioSource.PlayClipAtPoint(statitemSound, this.transform.position);
                    if(sword) {
                        myweapon.damage += 10;
                    }
                    else if(gun) {
                        myweapon.bullet_prefab.GetComponent<Bullet>().damage += 5;
                    }
                    break;
                case Item.ItemType.SPDUP:
                    AudioSource.PlayClipAtPoint(statitemSound, this.transform.position);
                    speed += 5;
                    break;
                case Item.ItemType.RANGEUP:
                    AudioSource.PlayClipAtPoint(statitemSound, this.transform.position);
                    if(sword) {
                        myweapon.transform.localScale += new Vector3(0.5f, 0.8f, 0.5f);
                    }
                    else if(gun) {
                        myweapon.range += 0.5f;
                    }
                    break;
                case Item.ItemType.RATEUP:
                    AudioSource.PlayClipAtPoint(statitemSound, this.transform.position);
                    if(sword) {
                        myweapon.rate -= 0.05f;
                    }
                    else if(gun) {
                        myweapon.rate -= 0.1f;
                    }
                    if(myweapon.rate < 0) myweapon.rate = 0;
                    break;
                case Item.ItemType.MAXHPUP:
                    AudioSource.PlayClipAtPoint(statitemSound, this.transform.position);
                    maxhealth += 50;
                    break;
                case Item.ItemType.DASHUP:
                    AudioSource.PlayClipAtPoint(statitemSound, this.transform.position);
                    manager.plusDash.SetActive(true);
                    maxDashCount++;
                    break;
            } 
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet") { 
            if(!isDamaged) { // 공격당하지 않은 경우
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage; // 플레이어와 부딪힌 물체(Bullet)의 데미지만큼 체력 감소
                bool isBossAtk = (other.name == "Boss Melee Area"); // 보스의 공격인가?
                AudioSource.PlayClipAtPoint(attackedSound, this.transform.position); // 공격 사운드 
                StartCoroutine("OnDamage", isBossAtk);
            }
            if(other.GetComponent<Rigidbody>() != null) {
                Destroy(other.gameObject); // 무적과 상관 없이 투사체는 무조건 없어짐
            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk) {
        isDamaged = true; // 공격을 받음 - 무적 상태
        foreach(MeshRenderer m in mat) { 
            m.material.color = Color.yellow; // 플레이어의 모든 Mesh를 노란색으로 변경합니다. 
        }
        if(isBossAtk) rigid.AddForce(transform.forward * -25, ForceMode.Impulse); // 보스의 공격을 받은 경우 뒤로 밀려납니다.
        if(health <= 0 && !isDead) { // 체력이 0 이하인 경우 죽음
            OnDie(); 
        }   
        yield return new WaitForSeconds(1f); // 무적 시간 1초
        isDamaged = false; // 공격을 받아 일시적으로 무적이 되었다가 풀림
        foreach(MeshRenderer m in mat) {
            m.material.color = Color.white; // 풀리면서 플레이어가 흰색으로 다시 돌아옴
        }
    }

    void OnDie() {
        ani.SetTrigger("doDie"); // 죽음 모션 수행
        GetComponent<Rigidbody>().isKinematic = true; // 물리 효과 영향을 받지 않게 함
        isDead = true; // 죽음 플래그 true
        manager.GameOver(); // 게임 매니저의 GameOver()함수 호출
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
