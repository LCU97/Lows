using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonMonoBehaviour<MonsterManager>
{
    GameObject m_monsterPrefab;
    GameObjectPool<MonsterController> m_monsterPool;
    [SerializeField]
    Camera m_uiCamera;
    [SerializeField]
    Transform m_hudPool;
    List<MonsterController> m_monsterList = new List<MonsterController>();

    public void CreateMonster()
    {
        var mon = m_monsterPool.Get();
        mon.InitMonster();
        m_monsterList.Add(mon);
    }
    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        if(m_monsterList.Remove(mon))
            m_monsterPool.Set(mon);
    }
    protected override void Onstart()
    {
        m_monsterPrefab =  Resources.Load<GameObject>("Prefab/Monsters/Monster2");
        m_monsterPool = new GameObjectPool<MonsterController>(5, () => 
        {
            var obj = Instantiate(m_monsterPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;            
            var mon = obj.GetComponent<MonsterController>();
            mon.SetMonster(m_uiCamera, m_hudPool);
            return mon;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            CreateMonster(); 
        }
        for(int i =0; i< m_monsterList.Count; i++)
        {
            m_monsterList[i].BehaviourProcess();
        }
    }
}
