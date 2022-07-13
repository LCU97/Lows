using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MonsterAnimController : AnimationController
{
    public enum Motion
    {
        None = -1,
        Idle,
        Run,
        Hit,
        Attack1,
        Die,
        Max
    }
    Motion m_state;

    public Motion GetAnimState()
    {
        return m_state;
    }
    StringBuilder m_sb = new StringBuilder();

    public void Play(Motion motion, bool isBlend = true)
    {
        m_sb.Append(motion);
        m_state = motion;
        Play(m_sb.ToString(), isBlend);
        m_sb.Clear();
    }

}
