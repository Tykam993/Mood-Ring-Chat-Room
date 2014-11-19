using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// This class is responsible for receiving a chat message string and deciding what
/// to do with them. Specifically, it will delimit the message and discard any common non-emotional
/// words. And then process em.
/// </summary>
public class ProcessChatMessage : MonoBehaviour {

    internal static string[] common_words = new string[]
    {
        "you", "it", "he", "they", "u", "she", "me", "i", "who", "what", "when", "where", "why"
    };

    public static void ProcessChat(string message)
    {
        message = message.Replace("'s", "");
        message = message.Replace("'", "");

        //Debug.Log("message before split: " + message);
                //string w = Utility.SanitizeString(message);
                //send it along!
                ProcessWord.Process(message);
            }

	
}
