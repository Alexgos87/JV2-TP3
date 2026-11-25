using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    public TextMeshProUGUI stopwatchText;
    public bool startOnAwake = true;

    private float elapsedTime = 0f;
    private bool isRunning;

    void Start()
    {
        isRunning = startOnAwake;
        UpdateDisplay();
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int hundredths = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
        stopwatchText.text = $"{minutes:00}:{seconds:00}.{hundredths:00}";
    }

    // Public methods, usable by UI buttons
    public void StartTimer() => isRunning = true;
    public void PauseTimer() => isRunning = false;

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateDisplay();
    }

    public float GetTime() => elapsedTime;
}