using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxController : MonoBehaviour
{
    [SerializeField]
    float m_duration;
    Animator m_animator;
    float m_time;
    void RemoveVfx()
    {
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        //m_animator = gameObject.GetComponent<Animator>();
        Invoke("RemoveVfx", m_duration); // 이거 하면 지정된 시간 뒤에 바로 함수를 사용함.
    }

    // Update is called once per frame
    /*void Update()
    {
        m_time += Time.deltaTime;
        if(m_time >= m_duration)
        {
            Destroy(gameObject);
        }
    }*/
    /*void Update()
    {
        var stateInfo= m_animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.normalizedTime >=1f)
        {
            RemoveVfx();
        }
    }*/
}
