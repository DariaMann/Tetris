using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countTest;
    [SerializeField] private int frameRange = 60;
    [SerializeField] private GameObject testPanel;
    
    private string[] _stringsFps =
    {
        "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
        "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
        "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
        "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
        "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
        "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
        "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
    };

    private int[] _fpsBuffer;
    private int _fpsBufferIndex;
    private int _countFPS;
    
    public static FpsCounter Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }
    
    void Update()
    {
        if (_fpsBuffer == null || frameRange != _fpsBuffer.Length)
        {
            InitializeBuffer();
        }

        UpdateBuffer();
        CalculateFps();
        countTest.text = "FPS: " + _stringsFps[Mathf.Clamp(_countFPS, 0 , 99)];
    }

    public void SetShowPanel(bool showPanel)
    {
        testPanel.SetActive(showPanel);
    }

    private void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }
        _fpsBuffer = new int[frameRange];
        _fpsBufferIndex = 0;
    }

    private void UpdateBuffer()
    {
        _fpsBuffer[_fpsBufferIndex++] = (int) (1f / Time.unscaledDeltaTime);
        if (_fpsBufferIndex >= frameRange)
        {
            _fpsBufferIndex = 0;
        }
    }
    
    private void CalculateFps()
    {
        int sum = 0;
        for (int i = 0; i < frameRange; i++)
        {
            sum += _fpsBuffer[i];
        }
        _countFPS = sum / frameRange;
    }
    
    public void OnUpdateSaveArea()
    {
        //todo: метод для отладки временный
        GUISaveArea[] allSaveAreas = FindObjectsOfType<GUISaveArea>();
        
        foreach (var area in allSaveAreas)
        {
            area.ApplySafeArea();
        }
    }
    
    public void OnResetData()
    {
        GameHelper.ResetData();
    }
}