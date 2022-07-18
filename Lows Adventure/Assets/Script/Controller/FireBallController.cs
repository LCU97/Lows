using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    [SerializeField]
    GameObject m_monsterObj;
    Monster2Controller m_monster;
    void OnParticleCollision(GameObject other) // ��ƼŬ ����Ʈ�� �ε����� ������ �̺�Ʈ
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
