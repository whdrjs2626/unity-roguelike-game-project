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
    public GameObject clearPanel;

    // 게임판넬 변수
    public Text scoreText; // 점수
    public Text stageText; // 스테이지
    public Text timerText; // 플레이타임
    public Text playerHPText; // 체력
    public Text playerAmmoText; // 탄약
    public Text playerCoinText; // 돈

    // 게임오버판넬 변수
    public Text overresultScore;

    // 게임클리어판넬 변수
    public Text clearresultScore;

    public GameObject[] Walls;
    public GameObject[] statUp_prefab;
    public GameObject[] reward;
    public GameObject[] Fireworks;

    public EnemySpawn[] spawner;


    public GameObject[] Dashs;
    public GameObject plusDash;

    public AudioClip clickSound;
    public AudioClip gameOverSound;
    public AudioClip gameClearSound;

    public AudioClip[] BGM;
    public AudioSource audioSource;

    public GameObject clearFirework;

    void Awake() {
        audioSource.clip = BGM[0];
        audioSource.loop = true;
        audioSource.Play();
    } 
    void Update() {
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
        if(player.sword) { // 망치인 경우
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
        AudioSource.PlayClipAtPoint(clickSound, this.transform.position);
        menuCamera.SetActive(false); // 게임 시작 카메라 비활성화
        mainCamera.SetActive(true); // 메인 카메라 활성화
        
        menuPanel.SetActive(false); // 게임 시작 화면 비활성화
        gamePanel.SetActive(true); // 게임 화면 활성화

        player.gameObject.SetActive(true); // 플레이어 활성화
        isStarted = true; // 시작됨
    }

    public void StageStart() {
        stage++;
        if(stage == 1) {
            StartCoroutine("nextBGM", 1);
            Walls[0].SetActive(true);
        }
        else if(stage == 2) {
            StartCoroutine("nextBGM", 2);
            Walls[2].SetActive(true);
        }
        else if(stage == 3) {
            StartCoroutine("nextBGM", 3);
            Walls[4].SetActive(true);
        }
        StopCoroutine("Battle");
        StartCoroutine("Battle");
    }

    IEnumerator Battle() {
        while(true) {
            yield return new WaitForSeconds(2.0f);
            if(spawner[stage-1].isStageOver) {
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
            StartCoroutine("nextBGM", 4);
            Walls[5].SetActive(false);
        }
        int rand = Random.Range(0, 5);
        if(rand == 4 && player.maxDashCount == 4) {}
        else Instantiate(statUp_prefab[rand], reward[stage-1].transform.position, Quaternion.identity);
        StartCoroutine("FireWork");
    }

    IEnumerator nextBGM(int i) {
        audioSource.Stop();
        yield return new WaitForSeconds(2.0f);
        audioSource.clip = BGM[i];
        audioSource.Play();
    }

    IEnumerator FireWork() {
        Fireworks[stage-1].SetActive(true);
        yield return new WaitForSeconds(8.0f);
        Fireworks[stage-1].SetActive(false);
    }

    public void GameOver() {
        AudioSource.PlayClipAtPoint(gameOverSound, player.transform.position);    
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        overresultScore.text = scoreText.text;
    }

    public void Restart() {
        AudioSource.PlayClipAtPoint(clickSound, this.transform.position);
        SceneManager.LoadScene(0);
    }

    public void GameClear() {
        AudioSource.PlayClipAtPoint(gameClearSound, player.transform.position);
        gamePanel.SetActive(false);
        clearPanel.SetActive(true);
        clearresultScore.text = scoreText.text;
        StartCoroutine("clearFireworks");
    }

    IEnumerator clearFireworks() {
        clearFirework.SetActive(true);
        yield return new WaitForSeconds(20.0f);
        clearFirework.SetActive(false);
    }
}
