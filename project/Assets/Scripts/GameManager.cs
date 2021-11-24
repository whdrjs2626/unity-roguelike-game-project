using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject menuCamera;
    public player player;
    public Boss boss;
    public int stage;
    public float timer;
    public bool isStarted = false;
    public int enemyCnt;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    // 게임판넬 변수
    public Text scoreText; // 점수
    public Text stageText; // 스테이지
    public Text timerText; // 플레이타임
    public Text playerHPText; // 체력
    public Text playerAmmoText; // 탄약
    public Text playerCoinText; // 돈
    //public RectTransform bossHPGroup; // 보스 체력
    //public RectTransform bossHPBar; // 보스 체력 바

    // 게임오버판넬 변수
    public Text resultScore;

    public GameObject[] Walls;
    public GameObject[] statUp_prefab;
    public GameObject[] reward;
    public GameObject[] Fireworks;

    public EnemySpawn[] spawner;


    public GameObject[] Dashs;
    public GameObject plusDash;

    void Update() {
        //GameObject = GameObject.Find("Boss(Clone)");
        if(isStarted) {
            timer += Time.deltaTime;
        }
    }
    void LateUpdate() {
        // 게임 판넬 UI
        int hour = (int)(timer / 3600);
        int minute = (int)((timer - hour * 3600) / 60);
        int second = (int)(timer % 60);

        // 상단 게임 정보 UI
        timerText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", minute) + ":" + string.Format("{0:00}", second);
        stageText.text = "STAGE " + stage;
        scoreText.text = string.Format("{0:n0}", player.score);
        
        // 플레이어 스탯 UI
        playerHPText.text = player.health + " / " + player.maxhealth;
        playerCoinText.text = string.Format("{0:n0}", player.coin);
        if(player.hammer) { // 망치인 경우
            playerAmmoText.text = "-";
        }
        else if(player.gun) { // 총인 경우
            playerAmmoText.text = player.curAmmo + " / " + player.maxAmmo;
        }
        /*
        // 보스 체력 UI
        if(boss != null) {
            bossHPGroup.anchoredPosition = Vector3.down * 50;
            bossHPBar.localScale = new Vector3((float)boss.HP / boss.maxHP, 1, 1);
        }
        else {
            bossHPGroup.anchoredPosition = Vector3.up * 500;
        }
        */
    }

    public void GameStart() {
        menuCamera.SetActive(false);
        mainCamera.SetActive(true);
        
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        isStarted = true;
    }

    public void StageStart() {
        stage++;
        if(stage == 1) {
            Walls[0].SetActive(true);
        }
        else if(stage == 2) {
            Walls[2].SetActive(true);
        }
        else if(stage == 3) {
            Walls[4].SetActive(true);
        }
        StopCoroutine("Battle");
        StartCoroutine("Battle");
    }

    IEnumerator Battle() {
        while(true) {
            yield return new WaitForSeconds(2.0f);
            if(spawner[stage-1].isStageOver) {
            //boss = null;
                StageEnd();
                break;
            }
        }
    }

    public void StageEnd() {
        if(stage == 1) {
            Walls[1].SetActive(false);
        }
        else if(stage == 2) {
            Walls[3].SetActive(false);
        }
        else if(stage == 3) {
            Walls[5].SetActive(false);
        }
        int rand = Random.Range(0, 5);
        Instantiate(statUp_prefab[rand], reward[stage-1].transform.position, Quaternion.identity);
        StartCoroutine("FireWork");
    }

    IEnumerator FireWork() {
        Fireworks[stage-1].SetActive(true);
        yield return new WaitForSeconds(5.0f);
        Fireworks[stage-1].SetActive(false);
    }

    public void GameOver() {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        resultScore.text = scoreText.text;
    }

    public void Restart() {
        SceneManager.LoadScene(0);
    }

    public void GameClear() {

    }

}
