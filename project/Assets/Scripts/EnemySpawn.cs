using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Transform[] points; // 몬스터 스폰 위치 변수 배열
    
    public GameObject[] monsterPrefab; // 스폰할 몬스터 배열

    public GameObject SpawnPoint; // 스폰 위치들을 포함하는 상위 오브젝트

    public GameManager manager; // 게임매니저 

    public float delay; // 몬스터 출현 주기
    public int maxMonster; // 몬스터 출현 수
    public bool isStageOver = false; // 스테이지 끝 플래그

    int curMonster; // 스폰한 몬스터 수
 
    void Start () {
        points = SpawnPoint.GetComponentsInChildren<Transform>();
        // 스폰 위치의 상위 오브젝트에서 자식들(스폰 위치)을 가져옴
        if(points.Length > 0)
        {
            StartCoroutine("spawnMonster"); // 몬스터 생성 코루틴
            if(manager.stage == 4) { // 보스 스테이지인 경우
                Instantiate(monsterPrefab[3], points[1].position, points[1].rotation); // 보스는 따로 스폰
            }
        }
    }

    IEnumerator spawnMonster()
    {
        while(!isStageOver)
        {
               if(curMonster < maxMonster) // 최대 수 만큼 소환할 때까지
                {
                    yield return new WaitForSeconds(delay); 
                    int i = Random.Range(1, points.Length); // 랜덤 포인트 난수
                    if(manager.stage == 1) { // 스테이지 1
                        Instantiate(monsterPrefab[0], points[i].position, points[i].rotation); // 몬스터 A만 생성
                    }
                    else if(manager.stage == 2) { // 스테이지 2
                        int rand = Random.Range(0, 2); // 몬스터 A, B만 생성
                        Instantiate(monsterPrefab[rand], points[i].position, points[i].rotation);
                    }
                    else if(manager.stage == 3) { // 스테이지 3
                        int rand = Random.Range(0, 3); // 몬스터 A, B, C만 생성
                        Instantiate(monsterPrefab[rand], points[i].position, points[i].rotation);
                    }
                    else if(manager.stage == 4) { // 스테이지 4
                        int rand = Random.Range(0, 3); // 보스는 미리 따로 소환, 몬스터 A, B, C 생성
                        Instantiate(monsterPrefab[rand], points[i].position, points[i].rotation);
                    }
                    curMonster++;
                }else
                {
                    yield return null;
                }
                yield return new WaitForSeconds(delay);
                int monsterCount = (int)GameObject.FindGameObjectsWithTag("Enemy").Length; // 현재 Hierarchy창에 생성된 적의 개수
                if(monsterCount <= 0) { // 적이 다 죽으면 isStageOver true
                    isStageOver = true;
                }
        }
    }
}
