using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator m_animator;
    Dictionary<string, float> m_dicComboInputTime = new Dictionary<string, float>();
    string m_prevMotion;
    public void CalculateComboInputTime()
    {
        var clips = m_animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].events.Length >= 2)
            {
                float attackTime = clips[i].events[0].time;
                float endFrameTime = clips[i].events[1].time;
                float result = (endFrameTime - attackTime);
                m_dicComboInputTime.Add(clips[i].name, result);
            }
        }
    }
    public float GetComboInputTime(string animName)
    {
        float time = 0f;
        m_dicComboInputTime.TryGetValue(animName, out time);
        return time;
    }
    public void Play(string animName, bool isBlend = true)
    {
        if(!string.IsNullOrEmpty(m_prevMotion))
        {
            m_animator.ResetTrigger(m_prevMotion);
            m_prevMotion = null;
        }
        if(isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
        m_prevMotion = animName;
    }
    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        CalculateComboInputTime();
    }

}
