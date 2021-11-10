using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed, timer;
    float h, v;
    bool walk;
    bool dash, isDash;
    int dashcount = 3, maxdashcount = 3;
    
    Animator ani;
    Vector3 moveVec;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake() 
    {
        rigid = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        GetInput();
        Move();
        Turn();
        Dash();
    }

    void GetInput() {        
        h = Input.GetAxisRaw("Horizontal"); // left, right button
        v = Input.GetAxisRaw("Vertical"); // up, down button
        walk = Input.GetButton("Walk"); // shift button
        dash = Input.GetButtonDown("Jump"); // space button
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
    
}

