using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{

    void OnParticleCollision(GameObject other) // ��ƼŬ ����Ʈ�� �ε����� ������ �̺�Ʈ
    {
        if(other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            Debug.Log("Hit!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
