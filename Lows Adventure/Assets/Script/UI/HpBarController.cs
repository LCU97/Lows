using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarController : MonoBehaviour
{
    [SerializeField]
    UIProgressBar m_hpBar;


    // Start is called before the first frame update
    void Start()
    {
        m_hpBar.value = 1f;
    }

}
