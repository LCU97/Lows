using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LoadSceneManager : DontDestroy<LoadSceneManager>
{
    public enum SceneState
    {
        None = -1,
        Title,
        Lobby,
        Game
    }
    [SerializeField]
    GameObject m_loadingObj;
    [SerializeField]
    UIProgressBar m_loadingBar;    
    [SerializeField]
    UILabel m_progressLabel;
    SceneState m_state;
    SceneState m_loadState = SceneState.None;
    AsyncOperation m_loadInfo;

    public void LoadSceneAsync(SceneState state)
    {
        if (m_loadState != SceneState.None) return;
        m_loadState = state;
        LoadSceneAsync(state.ToString());
    }
    public void LoadSceneAsync(string SceneName)
    {
        m_loadInfo = SceneManager.LoadSceneAsync(SceneName);
        m_loadingObj.SetActive(true);
    }
    void HideUI()
    {        
        m_loadingObj.SetActive(false);
    }

    // Start is called before the first frame update
    protected override void Onstart()
    {
        m_loadingObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (PopupManager.Instance.IsPopupOpen())
            {
                PopupManager.Instance.ClosePopup();
            }
            else
            {

                switch (m_state)
                {
                    case SceneState.Title:
                        PopupManager.Instance.OpenPopupOkCancel("안내", "정말로 게임을 종료?", ()=> {
#if UNITY_EDITOR
                            EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                        }, null, "예", "아니오");
                        break;
                    case SceneState.Lobby:
                        PopupManager.Instance.OpenPopupOkCancel("안내", "타이틀로 돌아가시겠습니까?", () =>
                        {
                            LoadSceneAsync(SceneState.Title);
                            PopupManager.Instance.ClosePopup();
                        }
                        
                        , null, "예", "아니오");
                        break;
                    case SceneState.Game:
                        PopupManager.Instance.OpenPopupOkCancel("안내", "게임을 종료하고 로비로 돌아가시겠습니까?",() =>
                        {
                            LoadSceneAsync(SceneState.Lobby);
                            PopupManager.Instance.ClosePopup();
                        }, null, "예", "아니오");
                        break;
                }
            }
        }
        if (m_loadInfo != null)
        {
            if (m_loadInfo.isDone)
            {
                m_loadInfo = null;
                m_loadingBar.value = 1f; 
                m_progressLabel.text = "100%";
                m_state = m_loadState;
                m_loadState = SceneState.None;
                Invoke("HideUI", 1f);
            }
            else
            {
                m_loadingBar.value = m_loadInfo.progress;
                m_progressLabel.text = Mathf.RoundToInt(m_loadInfo.progress * 100f).ToString()+"%";
            }
        }
    }
}
