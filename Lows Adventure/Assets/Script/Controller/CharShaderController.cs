using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShaderController : MonoBehaviour
{
    Renderer[] m_renderers;
    MaterialPropertyBlock m_mpBlock;
    Coroutine m_coroutine;
    [SerializeField]
    AnimationCurve m_dissloveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField]
    float m_dissolveDuration = 1f;
    float m_dissolveTime;

    public void InitCharShader()
    {
        StopAllCoroutines();
        m_mpBlock.SetColor("_RimColor", Color.black);
        m_mpBlock.SetFloat("_Duration", m_dissloveCurve.Evaluate(0f));
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);
        }
    }
    IEnumerator Coroutine_SetColor(Color color, float duration)
    {
        m_mpBlock.SetColor("_RimColor", color);
        for(int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);
        }
        yield return new WaitForSeconds(duration);
        m_mpBlock.SetColor("_RimColor", Color.black);
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);
        }
    }
    IEnumerator Coroutine_PlayDissolveCurve(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_dissolveTime = 0f;
        while(true)
        {
            yield return null;
            m_dissolveTime += Time.deltaTime / m_dissolveDuration;
            var value = m_dissloveCurve.Evaluate(m_dissolveTime);
            m_mpBlock.SetFloat("_Duration",value);
            for (int i = 0; i < m_renderers.Length; i++)
            {
                m_renderers[i].SetPropertyBlock(m_mpBlock);
            }
            if (m_dissolveTime > 1f)
            {
                yield break;
            }
        }
        
    }
    public void SetColor(Color color, float duration)
    {
        if(m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        } 
        m_coroutine = StartCoroutine(Coroutine_SetColor(color, duration));
    }
    public void SetDissolve(float duration, float delay = 0f)
    {
        m_dissolveDuration = duration;
        StartCoroutine(Coroutine_PlayDissolveCurve(delay));
    }
    // Start is called before the first frame update
    void Start()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
        m_mpBlock = new MaterialPropertyBlock(); 
    }

}
