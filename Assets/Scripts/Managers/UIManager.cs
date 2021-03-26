using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : NetworkBehaviour
{
    public Text m_MessageText;
    public Text m_CashText;
    public Text m_ShopText;
    private bool m_ShowShop;

    [ClientRpc]
    public void RpcSetMessage(string message){
        m_MessageText.text = message;
    }

    [Client]
    public void SetCashText(string text){
        m_CashText.text = text;
    }

    [Client]
    public void ToggleShop(){
        m_ShowShop = !m_ShowShop;
        if(m_ShowShop){
            m_ShopText.text = "Weapon\n\nMulti\nShoot\n(20$)\nPress K\n\nSniper\nShoot\n(15$)\nPress J";
        } else {
            m_ShopText.text = "";
        }
    }

}
