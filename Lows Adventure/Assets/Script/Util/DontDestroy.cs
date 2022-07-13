using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy<T> : MonoBehaviour where T : DontDestroy<T>
{
    public static T Instance { get; private set; }
    virtual protected void OnAwake()
    {

    }
    virtual protected void Onstart()
    {

    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == (T)this)
        {
            Onstart();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
