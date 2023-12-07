using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChangeName
{
    internal static class NameDatabase
    {
        const string DAT_ND_BROADCAST = "CHANGE_NAME_ND_Broadcast";
        const string SIG_SEND_NAMES = "CHANGE_NAME_SEND_NAMES";

        private static bool isHost = false;

        private static Dictionary<ulong, string> names = new Dictionary<ulong, string>();

        internal static void NDNetGetString(string data, string signature)
        {
            if (signature == SIG_SEND_NAMES)
            {
                var receivedNames = DecodeNames(data);

                // Check if the dictionaries are the same
                if (!names.SequenceEqual(receivedNames))
                {
                    names = receivedNames; // Update the names dictionary
                }
            }
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
    }
}
