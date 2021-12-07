using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public int[] itemPrice;
    public Text talkText;

    public player enterPlayer; // 플레이어

    public AudioClip buySound;
    public AudioClip failSound;

    public void Enter(player p)
    {
        AudioSource.PlayClipAtPoint(failSound, this.transform.position);
        enterPlayer = p; 
        shopPanel.SetActive(true);
    }

    public void Exit()
    {
        AudioSource.PlayClipAtPoint(failSound, this.transform.position);
        shopPanel.SetActive(false);
    }

    public void Buy(int index) {
        int price = itemPrice[index]; // 설정한 아이템 가격
        if(price > enterPlayer.coin) {
            AudioSource.PlayClipAtPoint(failSound, this.transform.position);
            StopCoroutine("Message"); // 만약 실행 중이면 끄고 시작
            StartCoroutine("Message");
            return; // 돈이 없으니 못삼
        }
        else {
            enterPlayer.coin -= price;
            AudioSource.PlayClipAtPoint(buySound, this.transform.position);
            if(index == 0) { // 체력 구매 
                enterPlayer.health += 10;
                if(enterPlayer.health > enterPlayer.maxhealth) enterPlayer.health = enterPlayer.maxhealth;
            }
            else if(index == 1) { // 총알 구매
                enterPlayer.maxAmmo += 30;
            }
            else if(index == 2) { // 공격력 증가
                enterPlayer.myweapon.damage += 5;
            }
        }
    }

    IEnumerator Message() {
        talkText.text = "돈이 부족하시네요...";
        yield return new WaitForSeconds(2.0f);
        talkText.text = "";
    }
}
