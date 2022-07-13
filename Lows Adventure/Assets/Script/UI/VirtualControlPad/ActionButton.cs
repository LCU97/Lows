using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public delegate void ButtonDel();

public class ActionButton : MonoBehaviour
{
    [SerializeField]
    UISprite m_darkGauge;
    [SerializeField]
    UISprite m_coolGauge;
    [SerializeField]
    UILabel m_remainTime;
    [SerializeField]
    float m_coolTime;
    float m_time;
    bool m_isReady;
    bool m_isPress;
    
    ButtonDel m_pressDelegate;
    ButtonDel m_releaseDelegate;
    StringBuilder m_sb = new StringBuilder();

    public void SetButton(float coolTime, ButtonDel pressDel, ButtonDel releaseDel)
    {
        m_darkGauge.enabled = false;
        m_coolGauge.enabled = false;
        m_remainTime.enabled = false;
        m_coolTime = coolTime;
        m_isReady = true;
        if(coolTime > 0f)
        {
            m_darkGauge.enabled = true;
            m_coolGauge.enabled = true;
            m_remainTime.enabled = true;
            m_darkGauge.fillAmount = 0f;
            m_coolGauge.fillAmount = 1f;
            m_remainTime.text = string.Empty;
        }
        m_pressDelegate = pressDel;
        m_releaseDelegate = releaseDel;
    }
    public void OnPressButton()
    {
        if (m_isReady)
        {
            m_isPress = true;
            if (m_pressDelegate != null)
            {
                m_pressDelegate();
            }
            if (m_coolTime > 0f)
            {
                m_isReady = false;  
            }
        }

    }
    public void OnReleaseButton()
    {
        if (m_isPress)
        {
            m_isPress = false;
            if (m_releaseDelegate != null)
            {
                m_releaseDelegate();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_isReady)
        {
            m_time += Time.deltaTime; 
            m_coolGauge.fillAmount =  m_time / m_coolTime;
            m_darkGauge.fillAmount = 1f - m_time / m_coolTime;
            m_sb.Append(Mathf.CeilToInt(m_coolTime - m_time));
            m_remainTime.text = m_sb.ToString();
            m_sb.Clear();
            if (m_time > m_coolTime)
            {
                m_time = 0f;
                m_darkGauge.fillAmount = 0f;
                m_coolGauge.fillAmount = 1f;
                m_remainTime.text = null;
                m_isReady = true;
            }
        }
    }
}
