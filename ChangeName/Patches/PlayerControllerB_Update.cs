using UnityEngine;
using GameNetcodeStuff;
using HarmonyLib;
using ChangeName;

namespace ChangeName.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    internal class PlayerControllerB_Update
    {
        private static float lastUpdateTime = 0;
        static void Postfix(PlayerControllerB __instance)
        {
            // Update this every 2 seconds
            if(lastUpdateTime + 2 < Time.time){
                // Host sends names
                if(__instance.IsHost){
                    // Check if there are any ids not present in the database
                    foreach(PlayerControllerB player in __instance.playersManager.allPlayerScripts){
                        if (!NameDatabase.HasId(NameDatabase.GetUsableIdFromPlayer(player))){
                            NameDatabase.SetName(NameDatabase.GetUsableIdFromPlayer(player), player.playerUsername);
                        }
                    }

                    if(NameDatabase.shouldSendNames){
                        NameDatabase.NDNetSendNames();
                        NameDatabase.shouldSendNames = false;
                    }
                }

                // Update names
                if(NameDatabase.shouldUpdateNames){
                    foreach(PlayerControllerB player in __instance.playersManager.allPlayerScripts){
                        string newName = NameDatabase.GetName(NameDatabase.GetUsableIdFromPlayer(player));
                        // Ship screen first
                        foreach(TransformAndName obj in StartOfRound.Instance.mapScreen.radarTargets){
                            Transform transform = obj.transform;
                            if(obj.name == player.playerUsername){
                                StartOfRound.Instance.mapScreen.ChangeNameOfTargetTransform(transform, newName);
                            }
                        }

                        // Username
                        player.playerUsername = newName;
                        
                        // HUD
                        player.usernameBillboardText.text = newName;

                    }

                    NameDatabase.shouldUpdateNames = false;
                }
                lastUpdateTime = Time.time;
            }
        }
    }
}


                // foreach (PlayerControllerB player in __instance.playersManager.allPlayerScripts)
                // {
                //     if(player.playerUsername == nameOfUserWhoTyped){
                //         player.playerUsername = extractedName;
                //         player.usernameBillboardText.text = extractedName;
                //         NameDatabase.UpdateName(player.playerSteamId, extractedName);
                //     }
                // }