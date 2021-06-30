using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    [SerializeField] private bool destroyOnLoad = false;

    public static T Instance
    {
        get { return instance; }
        set
        {
            if (instance == null)
            {
                instance = value;
            }
            else if (instance != value)
            {
                Destroy(value.gameObject);
            }
        }
    }

    public virtual void Awake()
    {
        Instance = this as T;
        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(this);
        }

    }
}

