using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;

public class Utility : MonoBehaviour
{

    private static Utility _instance;
    public static Utility Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<Utility>();
                _instance.gameObject.name = "Utility Object";
            }
            return _instance;
        }
    }

    /// <summary>
    /// Returns a sanitized version of the input string; the returned string
    /// will only contain letters or digits.
    /// </summary>
    /// <param name="lowerCase">Optional: Should the returned string be all lowercase?</param>
    /// <param name="strIn">The string to sanitize</param>
    /// <returns></returns>
    public static string SanitizeString(string strIn, bool lowerCase = true)
    {
        string result = strIn;

        if (lowerCase) { result = result.ToLower(); }

        //from stack overflow:
        //http://stackoverflow.com/questions/11395775/clean-the-string-is-there-any-better-way-of-doing-it
        return new System.String(result.Where(System.Char.IsLetterOrDigit).ToArray());
    }
}
