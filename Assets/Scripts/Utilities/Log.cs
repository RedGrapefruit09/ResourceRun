using UnityEngine;

/// <summary>
/// A small custom logging facade that uses Unity Logging (<see cref="Debug"/>) internally.
///
/// The main benefit of this is that inside all logging methods, an <code>#if UNITY_EDITOR</code> preprocessor
/// annotation is included, which allows excluding your non-visible logging from production builds (and reducing string
/// formatting overhead) without extra boilerplate required.
/// </summary>
public static class Log
{
    /// <summary>
    /// Logs a default-level (info) message to the Unity Console.
    /// </summary>
    /// <param name="message">The text of the message to be logged</param>
    public static void Info(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    /// <summary>
    /// Logs a warning message to the Unity Console.
    /// </summary>
    /// <param name="message">The text of the warning to be logged</param>
    public static void Warning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }

    /// <summary>
    /// Logs an error message to the Unity Console.
    /// </summary>
    /// <param name="message">The text of the error to be logged</param>
    public static void Error(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }
}