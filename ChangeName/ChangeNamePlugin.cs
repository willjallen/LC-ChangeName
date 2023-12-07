using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LC_API.ServerAPI;
namespace ChangeName
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class ChangeNamePlugin : BaseUnityPlugin
    {
        private const string GUID = "me.wallen.changename";
        private const string NAME = "Change Player Names";
        private const string VERSION = "0.1.0";

        private static ChangeNamePlugin Instance;

        void Awake()
        {
            Instance = this;

            LogInfo("Loading...");

            var harmony = new Harmony(GUID);
            harmony.PatchAll();

            Networking.GetString += NameDatabase.NDNetGetString;

            LogInfo("Loading Complete!");
        }

        #region logging
        internal static void LogDebug(string message) => Instance.Log(message, LogLevel.Debug);
        internal static void LogInfo(string message) => Instance.Log(message, LogLevel.Info);
        internal static void LogWarning(string message) => Instance.Log(message, LogLevel.Warning);
        internal static void LogError(string message) => Instance.Log(message, LogLevel.Error);
        internal static void LogError(Exception ex) => Instance.Log($"{ex.Message}\n{ex.StackTrace}", LogLevel.Error);
        private void Log(string message, LogLevel logLevel) => Logger.Log(logLevel, message);
        #endregion
    }
}
