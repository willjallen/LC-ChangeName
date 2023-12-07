
using GameNetcodeStuff;
using HarmonyLib;
using ChangeName;
using LC_API.ServerAPI;

namespace ChangeName.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "ArriveAtLevel")]
    internal class StartOfRound_ArriveAtLevel
    {
        static void Postfix(StartOfRound __instance)
        {
            // Networking.Broadcast("", NameDatabase.SIG_NEW_LEVEL);
        }
    }
}