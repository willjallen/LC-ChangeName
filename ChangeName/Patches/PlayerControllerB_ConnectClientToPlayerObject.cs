
using GameNetcodeStuff;
using HarmonyLib;
using ChangeName;
using LC_API.ServerAPI;

namespace ChangeName.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
    internal class PlayerControllerB_ConnectClientToPlayerObject
    {
        static void Postfix(PlayerControllerB __instance)
        {
            ChangeNamePlugin.LogDebug("Steam Disabled: " + GameNetworkManager.Instance.disableSteam);
            ChangeNamePlugin.LogDebug("Is Host: " + __instance.IsHost);
            ChangeNamePlugin.LogDebug("Player steam ID: " + __instance.playerSteamId.ToString());
            ChangeNamePlugin.LogDebug("Player client ID: " + __instance.playerClientId.ToString());
            ChangeNamePlugin.LogDebug("Actual client ID: " + __instance.actualClientId.ToString());
            ChangeNamePlugin.LogDebug("Player Username: " + __instance.playerUsername);
            Networking.Broadcast("", NameDatabase.SIG_PLAYER_JOIN);
        }
    }
}
