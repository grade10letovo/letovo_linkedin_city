using UnityEngine;
using System;
using System.IO;

public class LogToFile : MonoBehaviour
{
    private static StreamWriter logWriter;
    private static string logFilePath;

    private void Awake()
    {
#if UNITY_EDITOR
        string logDirectory = Path.Combine(Application.dataPath, "Logs");
#else
    string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
#endif


        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // Формируем путь к файлу с датой и временем
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = Path.Combine(logDirectory, $"log_{timeStamp}.txt");

        // Открываем файл для записи
        logWriter = new StreamWriter(logFilePath, true);
        logWriter.AutoFlush = true;

        // Подписываемся на события логирования
        Application.logMessageReceived += HandleLog;

        Debug.Log($"[LogToFile] 📝 Logging started at: {logFilePath}");
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;

        if (logWriter != null)
        {
            logWriter.Close();
            logWriter = null;
        }
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logWriter == null)
            return;

        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {logString}";
        logWriter.WriteLine(logEntry);

        // Для ошибок и исключений записываем стек-трейс
        if (type == LogType.Error || type == LogType.Exception)
        {
            logWriter.WriteLine(stackTrace);
        }
    }
}
