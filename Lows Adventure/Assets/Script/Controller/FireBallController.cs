using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{

    void OnParticleCollision(GameObject other) // 파티클 이펙트가 부딪히면 들어오는 이벤트
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
