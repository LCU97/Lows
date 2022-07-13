using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePad : SingletonMonoBehaviour<GamePad>
{
    [SerializeField]
    UISprite m_padBG;
    [SerializeField]
    UISprite m_padButton;
    Camera m_uiCamera;
    float m_maxDist = 0.264f;
    int m_fingerID;
    bool m_isDrag;
    Vector2 m_dir;

    public Vector2 GetAxis()
    {
        return m_dir;
    }
    protected override void Onstart()
    {
        m_fingerID = -1;
        m_uiCamera =  GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if(Input.GetMouseButtonDown(0))
        {            
            Ray ray = m_uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100f, 1 << LayerMask.NameToLayer("UI")))
            {
                if(rayHit.collider.transform == m_padBG.transform)
                {
                    var dir = rayHit.point - m_padBG.transform.position;
                    if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                    {
                        m_dir = dir;
                    }
                    else
                    {
                        m_dir = dir.normalized * m_maxDist;
                    }
                    m_padButton.transform.position = m_padBG.transform.position + (Vector3)m_dir;
                    m_dir /= m_maxDist;                    
                    m_isDrag = true;
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            m_isDrag = false;
            m_dir = m_padButton.transform.localPosition = Vector3.zero;
        }
        if(m_isDrag) // bg 내에 버튼 클릭 후 월드좌표를 이용하여 드래그 가능하게 만든다.
        {            
                var worldPos = m_uiCamera.ScreenToWorldPoint(Input.mousePosition);
                var dir = worldPos - m_padBG.transform.position;
                if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                {
                    m_dir = dir;                    
                }
                else
                {
                    m_dir = dir.normalized * m_maxDist;
                }
                m_padButton.transform.position = m_padBG.transform.position + (Vector3)m_dir;
                m_dir /= m_maxDist;            
        }
#elif UNITY_ANDROID || UNITY_IPHONE
        for (int i =0; i<Input.touchCount; i++)
        {
            if(Input.touches[i].phase == TouchPhase.Began)
            {
                Ray ray = m_uiCamera.ScreenPointToRay(Input.touches[i].position);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 100f, 1 << LayerMask.NameToLayer("UI")))
                {
                    if (rayHit.collider.transform == m_padBG.transform)
                    {
                        var dir = rayHit.point - m_padBG.transform.position;
                        if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                        {
                            m_dir = dir;

                        }
                        else
                        {
                            m_dir = dir.normalized * m_maxDist;
                        }
                        m_padButton.transform.position = m_padBG.transform.position + (Vector3)m_dir;
                        m_dir /= m_maxDist;
                        m_fingerID = Input.touches[i].fingerId;
                        m_isDrag = true;
                    }                    
                }
            }
            if((Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled) && Input.touches[i].fingerId == m_fingerID)
            {
                m_isDrag = false;
                m_dir = m_padButton.transform.localPosition = Vector3.zero;
                m_fingerID = -1;
            }
            if(m_isDrag)
            {
                if ( Input.touches[i].fingerId == m_fingerID)
                {
                    var worldPos = m_uiCamera.ScreenToWorldPoint(Input.touches[i].position);
                    var dir = worldPos - m_padBG.transform.position;
                    if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                    {
                        m_dir = dir;
                    }
                    else
                    {
                        m_dir = dir.normalized * m_maxDist;
                    }
                    m_padButton.transform.position = m_padBG.transform.position + (Vector3)m_dir;
                    m_dir /= m_maxDist;
                }
            }
        }

#endif
    }

}
