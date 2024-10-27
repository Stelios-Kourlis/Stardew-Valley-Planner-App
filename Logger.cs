using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour {
    private string logFilePath = "A:\\Home\\Game Related\\Stardew Valley Related\\SV Planner App/log.txt";

    void Awake() {

        // Subscribe to the log message received event
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy() {
        // Unsubscribe from the log message received event
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        // Format the log message
        string logMessage = $"{System.DateTime.Now}: {type} - {logString}\n{stackTrace}\n";
        // Debug.Log(logMessage);

        // Write the log message to the file
        File.AppendAllText(logFilePath, logMessage);
    }
}