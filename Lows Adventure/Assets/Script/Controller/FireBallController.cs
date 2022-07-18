using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    [SerializeField]
    GameObject m_monsterObj;
    Monster2Controller m_monster;
    void OnParticleCollision(GameObject other) // 파티클 이펙트가 부딪히면 들어오는 이벤트
    {
        if(other.CompareTag("Player"))
        {
            if (m_monsterObj.activeSelf)
            {
                float damage = 0f;
                var player = other.GetComponent<PlayerController>();
                Debug.Log("Hit!");
                AttackType attackType = m_monster.MonsterAttackProcess(player, out damage);
                player.SetDamagePlayer(attackType, damage);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_monster = m_monsterObj.GetComponent<Monster2Controller>();
    }

}
