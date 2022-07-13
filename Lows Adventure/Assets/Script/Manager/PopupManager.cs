using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ButtonDelegate();

public class PopupManager : DontDestroy<PopupManager>
{
    [SerializeField]
    GameObject m_popupOkCancelPrefab;
    [SerializeField]
    GameObject m_popupOkPrefab;
    List<GameObject> m_popupList = new List<GameObject>();
    int m_popupDepth = 1000;
    int m_popDepthGap = 10;

    public void OpenPopOk(string subject, string body, ButtonDelegate okBtnDel,  string okBtnText = "Ok")
    {
        var obj = Instantiate(m_popupOkPrefab);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        var panels = obj.GetComponentsInChildren<UIPanel>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].depth = m_popupDepth + m_popupList.Count * m_popDepthGap + i;
        }
        var popup = obj.GetComponent<Popup_Ok>();
        popup.SetUI(subject, body, okBtnDel, okBtnText);
        m_popupList.Add(obj);
    }
    public void OpenPopupOkCancel(string subject, string body, ButtonDelegate okBtnDel, ButtonDelegate cancelBtnDel, string okBtnText = "Ok", string cancelBtnText = "Cancel")
    {
        var obj = Instantiate(m_popupOkCancelPrefab);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        var panels = obj.GetComponentsInChildren<UIPanel>();
        for(int i = 0; i<panels.Length;i++)
        {
            panels[i].depth = m_popupDepth + m_popupList.Count * m_popDepthGap + i;
        }
        var popup =  obj.GetComponent<Popup_OkCancel>();
        popup.SetUI(subject, body, okBtnDel, cancelBtnDel, okBtnText, cancelBtnText);
        m_popupList.Add(obj);
    }
    public void ClosePopup()
    {        
        if (m_popupList.Count > 0)
        {
            Destroy(m_popupList[m_popupList.Count - 1]);
            m_popupList.RemoveAt(m_popupList.Count - 1);
        }
    }
    public bool IsPopupOpen()
    {
        return m_popupList.Count > 0;
    }
    // Start is called before the first frame update
    protected override void Onstart()
    {
        m_popupOkCancelPrefab = Resources.Load("Prefab/Popup/PopupOkCancel") as GameObject;
        m_popupOkPrefab = Resources.Load("Prefab/Popup/PopupOk") as GameObject;
    }
    int count;
    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (count % 2 == 0)
            {
                OpenPopupOkCancel("Notice", "안녕하쇼숑숑", null, null, "확인", "취소");
            }
            else
            {
                OpenPopOk("오류안내", "네트워크 상태가 불안정하여 게임을 종료합니다.", null, "Confirm");
            }
            count++;
        }

    }
}
 