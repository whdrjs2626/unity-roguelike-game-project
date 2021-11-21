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
    public bool isBattle;
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
    public RectTransform bossHPGroup; // 보스 체력
    public RectTransform bossHPBar; // 보스 체력 바

    // 게임오버판넬 변수
    public Text resultScore;

    public EnemySpawn spawner;

    void Update() {
        if(isBattle) {
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
            playerAmmoText.text = player.ammo + " / " + player.maxammo;
        }

        // 보스 체력 UI
        if(boss != null) {
            bossHPGroup.anchoredPosition = Vector3.down * 50;
            bossHPBar.localScale = new Vector3((float)boss.HP / boss.maxHP, 1, 1);
        }
        else {
            bossHPGroup.anchoredPosition = Vector3.up * 500;
        }
    }

    public void GameStart() {
        menuCamera.SetActive(false);
        mainCamera.SetActive(true);
        
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void StageStart() {
        //벽.SetActive(true);
        stage++;
        StartCoroutine("Battle");
    }

    IEnumerator Battle() {
        yield return new WaitForSeconds(2.0f);
        if(spawner.isStageOver) {
            //boss = null;
            StageEnd();
        }
    }

    public void StageEnd() {

    }

    public void GameOver() {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        resultScore.text = scoreText.text;
    }

    public void Restart() {
        SceneManager.LoadScene(0);
    }

}
