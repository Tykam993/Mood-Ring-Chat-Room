using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// This class is responsible for receiving a chat message string and deciding what
/// to do with them. Specifically, it will delimit the message and discard any common non-emotional
/// words. And then process em.
/// </summary>
public class ProcessChatMessage : MonoBehaviour {

    private static string[] common_words = new string[]
    {
        "you", "it", "he", "they", "u", "she", "me", "i", "who", "what", "when", "where", "why"
    };

    public static void ProcessChat(string message)
    {
        message = Utility.SanitizeString(message);
        message = message.Replace("'s", "");
        message = message.Replace("'", "");

        string[] words = message.Split(new char[] {' '});

        foreach (string word in words)
        {
            if (!common_words.Any(w => word == w))
            {
                //send it along!
                ProcessWord.Process(word);
            }
        }

    }

	
}
