using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_OkCancel : MonoBehaviour
{
    [SerializeField]
    UILabel m_subject;
    [SerializeField]
    UILabel m_body;
    [SerializeField]
    UILabel m_okBtnLabel;
    [SerializeField]
    UILabel m_cancelBtnLabel;
    ButtonDelegate m_okBtnDel;
    ButtonDelegate m_cancelBtnDel;
    public void SetUI(string subject, string body, ButtonDelegate okBtnDel, ButtonDelegate cancelBtnDel ,string okBtnText = "Ok", string cancelBtnText = "Cancel")
    {
        m_subject.text = subject;
        m_body.text = body;
        m_okBtnDel = okBtnDel;
        m_cancelBtnDel = cancelBtnDel;
        m_okBtnLabel.text = okBtnText;
        m_cancelBtnLabel.text = cancelBtnText;
    }
    public void OnPressOk()
    {
        if(m_okBtnDel != null)
        {
            m_okBtnDel();
        }
    }
    public void OnPressCancel()
    {
        if(m_cancelBtnDel != null)
        {
            m_cancelBtnDel();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }  


}
