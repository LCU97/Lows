using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Ok : MonoBehaviour
{
    [SerializeField]
    UILabel m_subject;
    [SerializeField]
    UILabel m_body;
    [SerializeField]
    UILabel m_okBtnLabel;    
    ButtonDelegate m_okBtnDel;    
    public void SetUI(string subject, string body, ButtonDelegate okBtnDel,string okBtnText = "Ok")
    {
        m_subject.text = subject;
        m_body.text = body;
        m_okBtnDel = okBtnDel;        
        m_okBtnLabel.text = okBtnText;       
    }
    public void OnPressOk()
    {
        if (m_okBtnDel != null)
        {
            m_okBtnDel();
        }       
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }
       

}
