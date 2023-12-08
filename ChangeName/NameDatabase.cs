using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using LC_API.ServerAPI;

namespace ChangeName
{
    internal static class NameDatabase
    {

        internal static bool shouldUpdateNames = false;
        internal static bool shouldSendNames = false;

        public const string SIG_BEGIN_UPDATE_NAMES = "CHANGE_NAME_BEGIN_UPDATE_NAMES";
        public const string SIG_UPDATE_NAME = "CHANGE_NAME_UPDATE_NAME";
        public const string SIG_END_UPDATE_NAMES = "CHANGE_NAME_END_UPDATE_NAMES";
        public const string SIG_PLAYER_JOIN = "CHANGE_NAME_PLAYER_JOIN";
        public const string SIG_NEW_LEVEL = "CHANGE_NAME_NEW_LEVEL";

        const string SIG_NEW_ROUND = "CHANGE_NAME_NEW_ROUND";

        // SteamIDs, names
        private static Dictionary<ulong, string> names = new Dictionary<ulong, string>();

        internal static string GetName(ulong id)
        {
            if (!names.ContainsKey(id))
            {
                ChangeNamePlugin.LogWarning("[NameDatabase.GetName] Failed to get name");
                return "";
            }
            else
            {
                return names[id];
            }
        }

        internal static void SetName(ulong id, string name)
        {
            if (!names.ContainsKey(id))
            {
                names.Add(id, name);
            }
            else
            {
                names[id] = name;
            }
        }

        internal static bool HasName(string name)
        {
            return names.ContainsValue(name);
        }

        internal static bool HasId(ulong id)
        {
            return names.ContainsKey(id);
        }

        internal static ulong GetUsableIdFromPlayer(PlayerControllerB player)
        {
            if (!GameNetworkManager.Instance.disableSteam)
            {
                return player.playerSteamId;
            }
            else
            {
                return player.playerClientId;
            }
        }

        internal static void UpdateName(string oldName, string newName)
        {
            if (!names.ContainsValue(oldName))
            {
                ChangeNamePlugin.LogWarning("[NameDatabase.UpdateName] Tried to update name " + oldName + " but it does not exist");
                ChangeNamePlugin.LogWarning(names.ToString());
            }
            else
            {
                var id = names.FirstOrDefault(x => x.Value == oldName).Key;
                names[id] = newName;
            }

            // The host should update its game state, since it will not receive broadcasts
            shouldUpdateNames = true;
        }

        internal static int NumberOfNames()
        {
            return names.Count;
        }

        internal static void NDNetGetString(string data, string signature)
        {
            switch (signature)
            {
                case SIG_PLAYER_JOIN:
                case SIG_NEW_ROUND:
                    if(StartOfRound.Instance.IsHost){
                        shouldSendNames = true;
                    }
                    break;

                case SIG_BEGIN_UPDATE_NAMES:
                    ChangeNamePlugin.LogInfo("[NameDatabase] Beginning name update process.");
                    break;

                case SIG_UPDATE_NAME:
                    var namePair = DecodeName(data);
                    if (namePair != null)
                    {
                        KeyValuePair<ulong, string> notNullPair = namePair.Value;
                        ChangeNamePlugin.LogInfo("[NameDatabase] New name received: " + notNullPair.Key.ToString() + " : " + notNullPair.Value);
                        names[notNullPair.Key] = notNullPair.Value;
                    }
                    break;

                case SIG_END_UPDATE_NAMES:
                    ChangeNamePlugin.LogInfo("[NameDatabase] End of name update process.");
                    shouldUpdateNames = true;
                    break;
            }
        }

        internal static void NDNetSendNames()
        {
            Networking.Broadcast(string.Empty, SIG_BEGIN_UPDATE_NAMES);

            foreach (var pair in names)
            {
                var encodedName = EncodeName(pair);
                Networking.Broadcast(encodedName, SIG_UPDATE_NAME);
            }

            Networking.Broadcast(string.Empty, SIG_END_UPDATE_NAMES);
        }

        private static string EncodeName(KeyValuePair<ulong, string> namePair)
        {
            return $"{namePair.Key}:{namePair.Value}";
        }

        private static KeyValuePair<ulong, string>? DecodeName(string encoded)
        {
            var keyValue = encoded.Split(':');
            if (keyValue.Length == 2)
            {
                ulong key;
                if (ulong.TryParse(keyValue[0], out key))
                {
                    return new KeyValuePair<ulong, string>(key, keyValue[1]);
                }
            }
            return null;
        }
        private static string EncodeNames(Dictionary<ulong, string> names)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var pair in names)
            {
                builder.Append($"{pair.Key}:{pair.Value};");
            }
            return builder.ToString();
        }

        private static Dictionary<ulong, string> DecodeNames(string encoded)
        {
            var pairs = encoded.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var dictionary = new Dictionary<ulong, string>();
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(':');
                dictionary[ulong.Parse(keyValue[0])] = keyValue[1];
            }
            return dictionary;
        }
        static string DictionaryToString(Dictionary<ulong, string> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pair in dictionary)
            {
                sb.AppendLine($"Key: {pair.Key}, Value: {pair.Value}");
            }
            return sb.ToString();
        }
    }

}
//  public void KickPlayer(int playerObjToKick)
//   {
//     if (!this.allPlayerScripts[playerObjToKick].isPlayerControlled && !this.allPlayerScripts[playerObjToKick].isPlayerDead || !this.IsServer)
//       return;
//     if (!GameNetworkManager.Instance.disableSteam)
//     {
//       ulong playerSteamId = StartOfRound.Instance.allPlayerScripts[playerObjToKick].playerSteamId;
//       if (!this.KickedClientIds.Contains(playerSteamId))
//         this.KickedClientIds.Add(playerSteamId);
//     }
//     NetworkManager.Singleton.DisconnectClient(this.allPlayerScripts[playerObjToKick].actualClientId);
//     HUDManager.Instance.AddTextToChatOnServer(string.Format("[playerNum{0}] was kicked.", (object) playerObjToKick));
//   }
