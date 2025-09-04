using System.Collections;
using UnityEngine;

public class InternetConnection : MonoBehaviour
{
    private Coroutine _coroutine;
    
    public static InternetConnection Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (_coroutine != null) return;

        _coroutine = StartCoroutine(InternetCoroutine());
    }
    
    private IEnumerator InternetCoroutine()
    {
        while (true)
        {
            bool haveInternet = false;
            yield return StartCoroutine(GameHelper.CheckInternetConnection(result =>
            {
                haveInternet = result;
            }));
            GameHelper.HaveInternet = haveInternet;
            yield return new WaitForSeconds(5f);
        }
    }
}