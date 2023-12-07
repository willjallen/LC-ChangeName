using GameNetcodeStuff;
using HarmonyLib;
using ChangeName;

namespace ChangeName.Patches
{
    [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
    internal class HUDManager_AddChatMessage
    {
        private static string lastChatMessage = "";
        static void Postfix(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped)
        {
            if (lastChatMessage == chatMessage){
                return;
            }
            lastChatMessage = chatMessage;
            
            ChangeNamePlugin.LogDebug(chatMessage);

            if(chatMessage.Contains("!name")){
                string extractedName = chatMessage.Substring(chatMessage.IndexOf("!name") + "!name".Length).Trim();
                // Check for duplicate names
                if(NameDatabase.HasName(extractedName)){
                  __instance.AddTextToChatOnServer("Duplicate names are not allowed!", -1);                      
                } else if (__instance.playersManager.IsHost){
                    NameDatabase.UpdateName(nameOfUserWhoTyped, extractedName);
                    NameDatabase.shouldSendNames = true; 
                    NameDatabase.shouldUpdateNames = true; // Since host will not get broadcast
                }
            }
        }
    }
}
