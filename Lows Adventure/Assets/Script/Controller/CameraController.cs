using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform m_target;
    [SerializeField]
    [Range(0f, 30f)] 
    float m_distance = 5f;
    [SerializeField]
    [Range(0f,30f)]
    float m_height = 3f;
    [SerializeField]
    [Range(-90f, 90f)]
    float m_angle = 30f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float m_speed = 0.1f;
    Transform m_prevTransfom;

    // Start is called before the first frame update
    void Start()
    {
        m_prevTransfom = transform;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_target.transform.position.x,
            Mathf.Lerp(m_prevTransfom.position.y, m_target.transform.position.y + m_height, m_speed * Time.deltaTime),
            Mathf.Lerp(m_prevTransfom.position.z, m_target.transform.position.z - m_distance, m_speed * Time.deltaTime));
        transform.rotation = Quaternion.Lerp(m_prevTransfom.rotation,Quaternion.Euler(m_angle,0f,0f), m_speed * Time.deltaTime);
        m_prevTransfom = transform;
    }
}
