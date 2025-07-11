using System;
using System.Collections.Generic;
using UnityEngine;
// using Firebase;
// using Firebase.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private DateTime _pauseTime;
    private bool _isInitialized = false;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // InitializeFirebase();
            // FirstLoad();
            // LogEvent(AnalyticType.session_start.ToString(), new Dictionary<string, object>
            // {
            //     { AnalyticType.timestamp.ToString(), DateTime.UtcNow.ToString("o") }
            // });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // void OnApplicationPause(bool pause)
    // {
    //     if (!_isInitialized) return;
    //     
    //     if (pause)
    //     {
    //         _pauseTime = DateTime.UtcNow;
    //         FirebaseAnalytics.LogEvent(AnalyticType.app_paused.ToString());
    //     }
    //     else
    //     {
    //         TimeSpan timeAway = DateTime.UtcNow - _pauseTime;
    //         FirebaseAnalytics.LogEvent(AnalyticType.app_resumed.ToString(), new Parameter[]
    //         {
    //             new Parameter(AnalyticType.pause_duration_sec.ToString(), (float)timeAway.TotalSeconds)
    //         });
    //
    //         // Например: считать, что это новая сессия, если игрок отсутствовал > 5 мин
    //         if (timeAway.TotalSeconds > 300)
    //         {
    //             FirebaseAnalytics.LogEvent(AnalyticType.session_restart.ToString(), new Parameter[]
    //             {
    //                 new Parameter(AnalyticType.pause_duration_sec.ToString(), (float)timeAway.TotalSeconds)
    //             });
    //         }
    //     }
    // }
    //
    // void OnEnable()
    // {
    //     Application.logMessageReceived += HandleLog;
    // }
    //
    // void OnDisable()
    // {
    //     Application.logMessageReceived -= HandleLog;
    // }

    private void FirstLoad()
    {
        // if (!PlayerPrefs.HasKey("device_info_logged"))
        // {
        //     var parameters = new Dictionary<string, object>()
        //     {
        //         { AnalyticType.platform.ToString(), Application.platform.ToString() },
        //         { AnalyticType.device_model.ToString(), SystemInfo.deviceModel },
        //         { AnalyticType.os_version.ToString(), SystemInfo.operatingSystem },
        //         { AnalyticType.language.ToString(), Application.systemLanguage.ToString() },
        //         { AnalyticType.app_version.ToString(), VersionInfo.Version },
        //         { AnalyticType.build.ToString(), VersionInfo.Build }
        //     };
        //     LogEvent(AnalyticType.device_info.ToString(), parameters);
        //
        //     PlayerPrefs.SetInt("device_info_logged", 1);
        // }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // if (!_isInitialized) return;
        //
        // if (type == LogType.Exception || type == LogType.Error)
        // {
        //     FirebaseAnalytics.LogEvent(AnalyticType.app_crash.ToString(), new Parameter[]
        //     {
        //         new Parameter(AnalyticType.message.ToString(), logString),
        //         new Parameter(AnalyticType.stack_trace.ToString(), stackTrace.Substring(0, Mathf.Min(100, stackTrace.Length))) // ограничим длину
        //     });
        // }
    }

    private void InitializeFirebase()
    {
// #if UNITY_EDITOR
//         Debug.Log("[Analytics] Firebase init skipped in Editor.");
//         return;
// #endif
//         
//         FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//         {
//             var status = task.Result;
//             if (status == DependencyStatus.Available)
//             {
//                 _isInitialized = true;
//                 Debug.Log("[Analytics] Firebase initialized.");
//                 LogAppOpen();
//                 if (GameHelper.IsAutentificate)
//                 {
//                     FirebaseAnalytics.SetUserId(GameHelper.GetPlayerID());
//                 }
//             }
//             else
//             {
//                 Debug.LogError("[Analytics] Firebase init failed: " + status);
//             }
//         });
    }

    private void LogAppOpen()
    {
        // LogEvent(FirebaseAnalytics.EventAppOpen);
    }

    public void LogEvent(string eventName)
    {
        // if (!_isInitialized) return;
        //
        // try
        // {
        //     FirebaseAnalytics.LogEvent(eventName);
        //     Debug.Log($"[Analytics] Event: {eventName}");
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError($"[Analytics] Failed to log event '{eventName}': {ex.Message}");
        // }
    }

    public void LogEvent(string eventName, Dictionary<string, object> parameters)
    {
        // if (!_isInitialized) return;
        //
        // if (parameters == null)
        //     parameters = new Dictionary<string, object>();
        //
        // List<Parameter> firebaseParams = new List<Parameter>();
        // foreach (var kvp in parameters)
        // {
        //     if (kvp.Value is string str)
        //         firebaseParams.Add(new Parameter(kvp.Key, str));
        //     else if (kvp.Value is int i)
        //         firebaseParams.Add(new Parameter(kvp.Key, i));
        //     else if (kvp.Value is float f)
        //         firebaseParams.Add(new Parameter(kvp.Key, f));
        //     else if (kvp.Value is double d)
        //         firebaseParams.Add(new Parameter(kvp.Key, d));
        // }
        //
        // try
        // {
        //     FirebaseAnalytics.LogEvent(eventName, firebaseParams.ToArray());
        //     Debug.Log($"[Analytics] Event: {eventName} with params: {parameters.Count}");
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError($"[Analytics] Failed to log event '{eventName}': {ex.Message}");
        // }
    }
}
