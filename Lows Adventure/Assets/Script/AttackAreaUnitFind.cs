using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaUnitFind : MonoBehaviour
{
    List<GameObject> m_unitList = new List<GameObject>();
    public List<GameObject> UnitList { get { return m_unitList; } }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {          
            m_unitList.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_unitList.Remove(other.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
}
