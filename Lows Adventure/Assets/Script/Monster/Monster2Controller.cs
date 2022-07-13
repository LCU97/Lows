using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2Controller : MonsterController
{
    GameObject m_fireBallPrefab;
    Transform m_dummyFireball;
    protected override void AnimEvent_Attack()
    {
        var dir = m_player.transform.position - transform.position;
        dir.y = 0f;
        var obj = Instantiate(m_fireBallPrefab);
        obj.transform.position = m_dummyFireball.position;
        obj.transform.forward = dir.normalized;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        m_fireBallPrefab = Resources.Load<GameObject>("Prefab/Effect/FX_Fireball_Shooting_Straight_01");
        m_dummyFireball = Util.FindChildObject(gameObject, "Dummy_FireBall").transform;
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
