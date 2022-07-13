using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HudController : MonoBehaviour
{
    [SerializeField]
    UILabel m_name;
    [SerializeField]
    UIProgressBar m_hpBar;
    [SerializeField]
    HUDText[] m_hudTexts;
    [SerializeField]
    UIFollowTarget m_followTarget;
    StringBuilder m_sb = new StringBuilder();

    public void SetHud(Camera uiCamera, Transform hudPool)
    {
        m_hpBar.value = 1f;
        m_followTarget.gameCamera = Camera.main;
        m_followTarget.uiCamera = uiCamera;
        transform.SetParent(hudPool);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true); // 로딩 시 인스턴스한다.
        gameObject.SetActive(false); // 일단 꺼놓고 ActiveUI함수로 SetDamege일 때 다시 켜준다.
    }
    public void InitHud() //초기화 불러올 떄 true한다.
    {
        m_hpBar.value = 1f;
        m_hpBar.alpha = 1f;
    }
    void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ActiveUI()
    {
        Show();
        if (IsInvoking("Hide"))
            CancelInvoke("Hide");
        Invoke("Hide", 3f);
    }
    public void DisplayDamage(AttackType type, float damage, float normalizeHp)
    {
        m_sb.Append(damage);
        switch(type)
        {
            case AttackType.Normal:
                m_hudTexts[0].Add(m_sb.ToString(), Color.white, 0f);
                break;
            case AttackType.Critical:
                m_hudTexts[1].Add(m_sb.ToString(), Color.red, 0f);
                break;
        }
        m_sb.Clear();
        m_hpBar.value = normalizeHp;
        if (normalizeHp <= 0f)
            m_hpBar.alpha = 0f;
    }
    // Start is called before the first frame update
    void Awake()
    {
        
    }
}
