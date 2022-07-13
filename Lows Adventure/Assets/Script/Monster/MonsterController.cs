using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum BehaviourState
    {
        Idle,
        Chase,
        Attack,
        Damaged,
        Die,
        Max
    }
    [Header("몬스터 능력치")]
    [SerializeField]
    Status m_status;
    [SerializeField]
    BehaviourState m_state;    
    protected PlayerController m_player;
    MonsterAnimController m_animCtr;
    NavMeshAgent m_navAgent;
    TweenMove m_tweenMove;
    CharShaderController m_shaderCtr;
    [SerializeField]
    HudController m_hudCtr;
    [SerializeField]
    Collider m_collider;
    GameObject m_fxHitPrefab;
    float m_idleDuration = 3f;
    float m_idleTime;
    float m_dieDuration = 3f;
    float m_dieTime;
    [SerializeField]
    float m_attackDist = 2f;
    [SerializeField]
    float m_dectectDist = 8f;
    float m_sqrAttackDist;
    float m_sqrDetectDist;
    int m_delayFrame;
    Coroutine m_coroutineDelayMotion;
    public bool IsDie { get { return m_state == BehaviourState.Die; } }
    public Status MyStatus { get { return m_status; } }

    #region Animation Event Methods
    protected virtual void AnimEvent_Attack()
    {
        if (m_player.m_isDodge) return;

        var dir = m_player.transform.position - transform.position;
        if (dir.sqrMagnitude <= m_sqrAttackDist)
        {
            var dot = Vector3.Dot(transform.forward, dir.normalized);
            if(dot >= 0.7071f)
            {
                var effect = Instantiate(m_fxHitPrefab);
                Destroy(effect, 1f);
                effect.transform.position = m_player.DummyHit.position; 
                effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, dir.normalized);
            }
        }
    }

    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    IEnumerator Coroutine_DelayMotion(int frame)
    {
        for(int i =0; i < frame; i++)
            yield return null;
        SetIdle(0f);
        m_delayFrame = 0;
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }
    void AnimEvent_HitFinished() 
    {        
        m_coroutineDelayMotion = StartCoroutine(Coroutine_DelayMotion(m_delayFrame));
    }
    #endregion

    #region Public Methods and Operators
    public void SetMonster(Camera uiCamera, Transform hudPool)
    {
        m_hudCtr.SetHud(uiCamera, hudPool);
    }
    public void InitMonster() // 몬스터 초기화
    {
        gameObject.SetActive(true);
        SetIdle(1f);
        m_status.hp = m_status.hpMax;
        m_hudCtr.InitHud();
        
    }
    #endregion
    IEnumerator Coroutine_SetDestination(int frame, Transform target)
    {
        while(m_state == BehaviourState.Chase)
        {
            for(int i =0; i < frame; i++)
            {
                yield return null;
            }
            m_navAgent.SetDestination(target.position);
        }

    }
    #region public Methods
    public void SetDamage(AttackType attackType,float damage, SkillData skilldata)
    {
        if(IsDie) return;
        m_hudCtr.ActiveUI();
        m_status.hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, m_status.hp / (float)m_status.hpMax);
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        m_shaderCtr.SetColor(Color.yellow, 1f);
        if (m_coroutineDelayMotion != null)
        {
            StopCoroutine(m_coroutineDelayMotion);
            m_coroutineDelayMotion = null;
        }
        m_animCtr.Play(MonsterAnimController.Motion.Hit, false);
        SetState(BehaviourState.Damaged);        
        m_delayFrame = skilldata.delayFrame;
        if (skilldata.knockBack > 0f)
        {
            var duration = SkillData.maxKnockBackDuration * (skilldata.knockBack / SkillData.maxKnockBackDist);
            m_tweenMove.Play(transform.position, transform.position + (transform.position - m_player.transform.position).normalized * skilldata.knockBack, duration);
        }
        if(m_status.hp <= 0f)
        {
            SetState(BehaviourState.Die);
            m_animCtr.Play(MonsterAnimController.Motion.Die);
            m_shaderCtr.SetDissolve(4f, 1.6f);
        }
    }

    #endregion
    void SetState(BehaviourState state)
    {
        m_state = state;
    }    
    void SetIdleDuration(float duration) // 공격거리에 들어왔을 때 몇초 후 공격을 계시할것인지. 초기 발견시 5초후 공격하지만, Chase 이후에는 해당 함수를 이용 원하는 IdleTime 후 재공격하도록 설정.
    {
        m_idleTime = m_idleDuration - duration;
        if (m_idleTime < 0f) m_idleTime = 0f;
    }
    void SetIdle(float duration)
    {
        m_navAgent.ResetPath();
        m_navAgent.isStopped = false;
        SetState(BehaviourState.Idle);
        m_animCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);
    }
    bool IsInSetArea(float curDist, float targetDist)
    {
         if(Mathf.Approximately(curDist, targetDist) || curDist < targetDist)
        {
            return true;
        }
        return false;
    }
    bool FindTarget(Transform target, float distance)
    {
        var dir = target.position - transform.position;
        dir.y = 0f;
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up * 1f, dir.normalized, out hit, distance, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Player")))
        {           
            if(hit.collider.CompareTag("Player"))
            {
                return true;                
            }            
        }
        return false;
    }
    
    public void BehaviourProcess()
    {
        float dist = 0f;
        switch(m_state)
        {            
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if(m_idleTime>m_idleDuration)
                {
                    m_idleTime = 0f;
                    m_navAgent.isStopped = false;
                    if (FindTarget(m_player.transform, m_attackDist))
                    {
                        var dir = m_player.transform.position - transform.position;
                        dir.y = 0f;
                        transform.forward = dir.normalized;
                        SetState(BehaviourState.Attack);
                        m_animCtr.Play(MonsterAnimController.Motion.Attack1);
                        return;
                    }
                    if(FindTarget(m_player.transform, m_dectectDist))
                    {
                        SetState(BehaviourState.Chase);
                        m_animCtr.Play(MonsterAnimController.Motion.Run);
                        StartCoroutine(Coroutine_SetDestination(5, m_player.transform));
                        m_navAgent.stoppingDistance = m_attackDist;
                        return;
                    }
                }
                break;
            case BehaviourState.Attack:
                break;
            case BehaviourState.Chase:
                //m_navAgent.SetDestination(m_player.transform.position);
                dist = (m_player.transform.position - transform.position).sqrMagnitude; 
                if (IsInSetArea(dist, m_sqrAttackDist))
                {
                    m_navAgent.isStopped = true;
                    SetIdle(0f);
                    return;
                }
                    break;
            case BehaviourState.Die:
                m_dieTime += Time.deltaTime;
                if(m_dieTime >= m_dieDuration)
                {
                    MonsterManager.Instance.RemoveMonster(this);
                    m_dieTime = 0f;
                }
                break;
        }
    }
    AttackType MonsterAttackProcess(PlayerController player, out float damage)
    {
        AttackType type = AttackType.Normal;
        var plusdamage = Random.Range(0, 10f);
        damage = 0f;
        damage = CalculationDamage.NormalDamage(m_status.attack, 0f, player.MyStatus.defence) + plusdamage;
        return type;
    }
    void InitStatus()
    {
        m_status = new Status(500, 0f, 0f, 50f, 20f);
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        InitStatus();
        m_sqrAttackDist = Mathf.Pow(m_attackDist, 2f);
        m_sqrDetectDist = Mathf.Pow(m_dectectDist, 2f);
        m_animCtr = GetComponent<MonsterAnimController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_tweenMove = GetComponent<TweenMove>();
        m_shaderCtr = GetComponent<CharShaderController>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_fxHitPrefab = Resources.Load("Prefab/Effect/FX_Attack01_01") as GameObject;
    }

    // Update is called once per frame
    /*void Update()
    {
        BehaviourProcess();
    }*/
}
