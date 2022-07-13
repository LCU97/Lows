using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerAnimController : AnimationController
{
    public enum Motion
    {
        None = -1,
        Idle,
        Run,
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        Skill1,
        Skill2,
        Roll,
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
