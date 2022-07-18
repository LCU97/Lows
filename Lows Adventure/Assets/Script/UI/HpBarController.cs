using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HpBarController : MonoBehaviour
{
    [SerializeField]
    UIProgressBar m_hpBar;
    [SerializeField]
    PlayerController m_player;
    StringBuilder m_sb = new StringBuilder();

    public void DisplayDamage(float damage, float nomalizedHp)
    {
        m_sb.Append(-Mathf.CeilToInt(damage));
        m_player.m_hudText.Add(m_sb.ToString(), Color.blue, 0f);
        m_sb.Clear();
        m_hpBar.value = nomalizedHp;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_hpBar.value = 1f;
    }

}
