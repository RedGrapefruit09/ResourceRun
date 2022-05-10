using System;
using System.IO;
using UnityEngine;

public class FileLogger : MonoBehaviour
{
    private string _path;

    private void Start()
    {
        var date = DateTime.Now.ToString("yyyy-M-d-h-mm-ss-tt");
        var directoryPath = $"{Application.persistentDataPath}/Logs";
        Directory.CreateDirectory(directoryPath);
        _path = $"{directoryPath}/{date}.txt";

        Application.logMessageReceived += LogToFile;
    }

    private void LogToFile(string message, string stackTrace, LogType logType)
    {
        var writer = File.AppendText(_path);
        var currentTime = DateTime.Now.ToString("G");
        writer.Write($"[{logType}] [{currentTime}]: {message}\n");
        writer.Close();
    }
}