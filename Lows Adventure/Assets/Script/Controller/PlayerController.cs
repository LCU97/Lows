using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("주인공 능력치")]
    [SerializeField]
    Status m_status;
    PlayerAnimController m_animCtr;
    List<PlayerAnimController.Motion> m_comboList = new List<PlayerAnimController.Motion>() { PlayerAnimController.Motion.Attack1, PlayerAnimController.Motion.Attack2, PlayerAnimController.Motion.Attack3, PlayerAnimController.Motion.Attack4 };
    Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>(); // 커맨드 저장공간
    Dictionary<PlayerAnimController.Motion, SkillData> m_skillTable = new Dictionary<PlayerAnimController.Motion, SkillData>();
    AttackAreaUnitFind[] m_attackArea;
    [SerializeField]
    HUDText m_hudText;
    [SerializeField]
    GameObject m_attackAreaObj;
    NavMeshAgent m_navAgent;
    Transform m_dummyHit;
    GameObject m_fxHitPrefab;
    Vector3 m_dir;    
    [SerializeField]
    float m_speed = 1f;    
    bool m_isPressAttack;
    bool m_isCombo;
    int m_comboIndex;
    public bool m_isDodge; 
    public Transform DummyHit { get { return m_dummyHit; } }
    public Status MyStatus { get { return m_status; } }

    public bool IsAttack { get {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack1 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack2 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack3 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack4 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Skill1 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Skill2)
                return true;
            return false;    
                } }



    #region Animation Event Methods
    void AnimEvent_Attack()
    {
        SkillData skillData;
        float damage = 0f;
        if (m_skillTable.TryGetValue(m_animCtr.GetAnimState(), out skillData))
        {
            var unitList = m_attackArea[skillData.attackArea].UnitList;
            for (int i = 0; i < unitList.Count; i++)
            {
                var mon = unitList[i].GetComponent<MonsterController>();
                var dummy = Util.FindChildObject(unitList[i], "Dummy_Hit");
                if (dummy != null && mon != null)
                {
                    AttackType type = AttackProcess(mon, skillData, out damage);
                    mon.SetDamage(type,damage,skillData);
                    var effect = Instantiate(m_fxHitPrefab);
                    Destroy(effect, 1f);
                    effect.transform.position = dummy.transform.position;
                    effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, (transform.position - unitList[i].transform.position).normalized);
                }
            }
        }
    } 
    void AnimEvent_AttackFinished()
    {
        bool isCombo = false;

        if (m_isPressAttack)
        {
            isCombo = true;
        }
        if(m_keyBuffer.Count == 1) // 입력이 들어왔다. 그리고 콤보를 여러번 막 누르는게 아니라 타이밍 맞춰서 한 번만 눌렀다.
        {
            var key = m_keyBuffer.Dequeue();
            if (key == KeyCode.C)
                isCombo = true;
        }
        else if(m_keyBuffer.Count >1)
        {
            ReleaseKeyBuffer();
            isCombo = false;
        }
        if(isCombo)
        {
            m_comboIndex++;
            if(m_comboIndex >= m_comboList.Count)
            {
                m_comboIndex = 0;
            }
            m_animCtr.Play(m_comboList[m_comboIndex]);            
        }
        else
        {
            m_animCtr.Play(PlayerAnimController.Motion.Idle);
            m_comboIndex = 0;
        }
    }
    #endregion
    AttackType AttackProcess(MonsterController mon, SkillData skillData, out float damage)
    {
        AttackType type = AttackType.Normal;
        damage = 0f;
        damage = CalculationDamage.NormalDamage(m_status.attack, skillData.attack, mon.MyStatus.defence);
        if(CalculationDamage.CriticalDecision(MyStatus.criRate))
        {
            type = AttackType.Critical;
            damage = CalculationDamage.CriticalDamage(damage, MyStatus.criAttack);
        }
        return type;
    }
    void SetDamagePlayer(AttackType attackType, float damage)
    {
        m_status.hp -= Mathf.CeilToInt(damage);

    }
    Vector3 GetPadDir()
    {
        var padDir = GamePad.Instance.GetAxis();
        Vector3 dir = Vector3.zero;
        if(padDir.x < 0.0f)
        {
            dir += Vector3.left * Mathf.Abs(padDir.x); 
        }
        if(padDir.x > 0.0f)
        {
            dir += Vector3.right * padDir.x;
        }
        if(padDir.y  < 0.0f)
        {
            dir += Vector3.back * Mathf.Abs(padDir.y);
        }
        if (padDir.y > 0.0f)
        {
            dir += Vector3.forward * padDir.y;
        }
        return dir;
    }
    void InitSkillData()
    {
        m_skillTable.Add(PlayerAnimController.Motion.Attack1, new SkillData() { attackArea = 0, knockBack = 0.2f, delayFrame = 0 , attack = 0f});
        m_skillTable.Add(PlayerAnimController.Motion.Attack2, new SkillData() { attackArea = 1, knockBack = 0.3f, delayFrame = 7, attack = 5f });
        m_skillTable.Add(PlayerAnimController.Motion.Attack3, new SkillData() { attackArea = 2, knockBack = 0.4f, delayFrame = 15, attack = 10f });
        m_skillTable.Add(PlayerAnimController.Motion.Attack4, new SkillData() { attackArea = 3, knockBack = 2f, delayFrame = 30, attack = 20f });
        m_skillTable.Add(PlayerAnimController.Motion.Skill1, new SkillData() { attackArea = 2, knockBack = 2f, delayFrame = 30, attack = 70f, coolTime = 3f });
        m_skillTable.Add(PlayerAnimController.Motion.Skill2, new SkillData() { attackArea = 3, knockBack = 1.5f, delayFrame = 30, attack = 100f, coolTime = 3f });
        m_skillTable.Add(PlayerAnimController.Motion.Roll, new SkillData() { attackArea = 0, knockBack = 0f, delayFrame = 0, attack = 0f, coolTime = 8f });

    }
    void InitStatus()
    {
        m_status = new Status(1000, 20f, 150f, 50f, 30f);
    }
    void ReleaseKeyBuffer()
    {
        m_keyBuffer.Clear();
    }
    public void OnPressAttack()
    {
        if (IsAttack)
        {
            if (IsInvoking("ReleaseKeyBuffer"))
            {
                CancelInvoke("ReleaseKeyBuffer");
            }
            float time = m_animCtr.GetComboInputTime(m_comboList[m_comboIndex].ToString());
            Invoke("ReleaseKeyBuffer", time);
            m_keyBuffer.Enqueue(KeyCode.C);
        }
        if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Idle || m_animCtr.GetAnimState() == PlayerAnimController.Motion.Run)
        {
            m_animCtr.Play(PlayerAnimController.Motion.Attack1);
        }
        m_isPressAttack = true;
    }
    public void OnReleaseAttack()
    {
        m_isPressAttack = false;
    }
    public void OnPressSkill1()
    {
        m_animCtr.Play(PlayerAnimController.Motion.Skill1, false);
    }
    public void OnPressSkill2()
    {
        m_animCtr.Play(PlayerAnimController.Motion.Skill2, false);
    }
    public void OnPressSkill3()
    {
        m_animCtr.Play(PlayerAnimController.Motion.Roll, false);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_dummyHit = Util.FindChildObject(gameObject, "Dummy_Hit").transform;
        m_animCtr = GetComponent<PlayerAnimController>();        
        m_navAgent = GetComponent<NavMeshAgent>();
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackAreaUnitFind>(); // attackAreaObj의 자식 오브젝트 중 AttackAreaUnitFind 스크립트를 가지고 있는 오브젝트를 찾아서 attackArea의 배열에 넣는다.
        m_fxHitPrefab = Resources.Load("Prefab/Effect/FX_Attack01_01") as GameObject;
        InitSkillData();
        InitStatus();        
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Main, 0f, OnPressAttack, OnReleaseAttack);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill1, m_skillTable[PlayerAnimController.Motion.Skill1].coolTime, OnPressSkill1, null);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill2, m_skillTable[PlayerAnimController.Motion.Skill2].coolTime, OnPressSkill2, null);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill3, m_skillTable[PlayerAnimController.Motion.Roll].coolTime, OnPressSkill3, null);
        
    }


    // Update is called once per frame
    void Update()
    {
        var padDir = GetPadDir();
        m_dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_hudText.Add((-Random.Range(5, 200)).ToString(), Color.red, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_hudText.Add("+ " + Random.Range(5, 200).ToString(), Color.green, 2f);
        }
        if (m_dir == Vector3.zero && padDir != Vector3.zero)
        {
            m_dir = padDir;
        }
        if (m_dir != Vector3.zero && !IsAttack)
        {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Idle)
            {
                m_animCtr.Play(PlayerAnimController.Motion.Run);
            }
            transform.forward = m_dir;            
        }
        else
        {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Run)
            {
                m_animCtr.Play(PlayerAnimController.Motion.Idle);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Main);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ActionButtonManager.Instance.OnReleaseButton(ActionButtonManager.ButtonType.Main);
        }        
        if(Input.GetKeyDown(KeyCode.Z))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill2);
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill3);
        } 
        if (!IsAttack && m_animCtr.GetAnimState() != PlayerAnimController.Motion.Roll)
        {
            m_navAgent.Move(m_dir * m_speed * Time.deltaTime);
        }
    }
}
