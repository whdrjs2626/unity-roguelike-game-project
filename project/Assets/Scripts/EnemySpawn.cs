using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
     //몬스터가 출현할 위치를 담을 배열
    public Transform[] points;
    //몬스터 프리팹을 할당할 변수
    public GameObject[] monsterPrefab;

    public GameObject SpawnPoint;

    public GameManager manager;

    //몬스터를 발생시킬 주기
    public float createTime;
    //몬스터의 최대 발생 개수
    public int maxMonster = 10;
    //게임 종료 여부 변수
    public bool isStageOver = false;

    int curMonster;
    //bool isBroken = false;
 
    // Use this for initialization
    void Start () {
        //Hierarchy View의 Spawn Point를 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        points = SpawnPoint.GetComponentsInChildren<Transform>();
 
        if(points.Length > 0)
        {
            //몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());

            //Invoke("setGameOver", 5f);
        }
    }

    void setGameOver() {
        while( !isStageOver ) {
            
        }
    }
 
    IEnumerator CreateMonster()
    {
        //게임 종료 시까지 무한 루프
        while( !isStageOver )
        {
               //현재 생성된 몬스터 개수 산출
               //int monsterCount = (int)GameObject.FindGameObjectsWithTag("Enemy").Length;
               
               if(curMonster < maxMonster)
                {
                    //몬스터의 생성 주기 시간만큼 대기
                    yield return new WaitForSeconds(createTime);
 
                    //불규칙적인 위치 산출
                    int idx = Random.Range(1, points.Length);
                    //몬스터의 동적 생성
                    if(manager.stage == 1) {
                        Instantiate(monsterPrefab[0], points[idx].position, points[idx].rotation);
                    }
                    else if(manager.stage == 2) {
                        int rand = Random.Range(0, 2);
                        Instantiate(monsterPrefab[rand], points[idx].position, points[idx].rotation);
                    }
                    else if(manager.stage == 3) {
                        int rand = Random.Range(0, 3);
                        Instantiate(monsterPrefab[rand], points[idx].position, points[idx].rotation);
                    }

                    curMonster++;
                }else
                {
                    yield return null;
                }
                yield return new WaitForSeconds(createTime);
                int monsterCount = (int)GameObject.FindGameObjectsWithTag("Enemy").Length;
                if(monsterCount <= 0) {
                    isStageOver = true;
                }
        }
    }
}