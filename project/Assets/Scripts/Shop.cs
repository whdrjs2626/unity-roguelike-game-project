using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup; // UI
    
    public int[] itemPrice;
    public Text talkText;

    public player enterPlayer; // 플레이어

    

    public void Enter(player p)
    {
        enterPlayer = p;
        uiGroup.anchoredPosition = Vector3.zero;    
    }

    public void Exit()
    {
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index) {
        //
        //

        int price = itemPrice[index];
        if(price > enterPlayer.coin) {
            StopCoroutine("Talk"); // 만약 실행 중이면 끄고 시작
            StartCoroutine("Talk");
            return; // 돈이 없으니 못삼
        }
        else {
            enterPlayer.coin -= price;
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

    IEnumerator Talk() {
        talkText.text = "돈이 부족하시네요...";
        yield return new WaitForSeconds(2.0f);
        talkText.text = "";
    }
}
