using GameNetcodeStuff;
using HarmonyLib;

namespace ChangeName.Patches
{
    [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
    internal class HUDManager_AddChatMessage
    {
        static void Postfix(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped)
        {
            ChangeNamePlugin.LogInfo(chatMessage);
            ChangeNamePlugin.LogInfo(nameOfUserWhoTyped);
            if(chatMessage.Contains("!name")){
                string extractedName = chatMessage.Substring(chatMessage.IndexOf("!name") + "!name".Length).Trim();
                ChangeNamePlugin.LogInfo(extractedName);

                foreach (PlayerControllerB player in __instance.playersManager.allPlayerScripts)
                {
                    if(player.playerUsername == nameOfUserWhoTyped){
                        player.playerUsername = extractedName;
                        player.usernameBillboardText.text = extractedName;
                    }
                }

            }
        }
    }
}
