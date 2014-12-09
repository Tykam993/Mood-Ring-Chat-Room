//This code was written following Brent Farris' "Simple Chat System" tutorial on Youtube.com
//https://www.youtube.com/watch?v=yS_kX8RrovQ
using UnityEngine;
using System.Collections.Generic;

public class ProcChat2 : MonoBehaviour
{

	int GUIWidth = 350, GUIHeight = 500;
	public string userName = "Chat_User";
	public List<string> chatHistory = new List<string> ();
	private string currentMessage = string.Empty;
	public Vector2 scrollPosition;
	public int sadness = 0;
	public int happiness = 0;
	public int anger = 0;
	public int fear = 0;
	public int disgust = 0;
	public int surprise = 0;

	private void OnGUI ()
	{
		GUI.color = Color.white;

			if (!Network.isClient) {
					return;
			}

		GUILayout.Space (20);

		GUILayout.BeginHorizontal (GUILayout.Width (GUIWidth));

		currentMessage = GUILayout.TextField (currentMessage, GUILayout.Width(Screen.width/3), 
		                                      GUILayout.Height(Screen.height/15));
		GUILayout.EndHorizontal ();


		//GUILayout.Space (20);
		GUILayout.BeginHorizontal (GUILayout.Width (GUIWidth));
		GUI.color = Color.white;
		if (GUILayout.Button ("   Send   ", GUILayout.Width(Screen.width/3), 
		                      GUILayout.Height(Screen.height/15)) || (Event.current.keyCode == KeyCode.Return)) {

			if (!string.IsNullOrEmpty (currentMessage.Trim ())) {
				networkView.RPC ("ChatMessage", RPCMode.AllBuffered, new object[] {userName + ":  " + currentMessage });

				//in here process the current message sent!
				ProcessChatMessage.ProcessChat (currentMessage);

				currentMessage = string.Empty;
			}


		}

		GUILayout.EndHorizontal ();

		//GUILayout.Space (20);
			
		GUILayout.BeginHorizontal (GUILayout.Width (GUIWidth));

		userName = GUILayout.TextField (userName, GUILayout.Width(Screen.width/3), 
		                                GUILayout.Height(Screen.height/15));

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal (GUILayout.Width (GUIWidth));
		GUI.color = Color.white;
		if (GUILayout.Button ("Change", GUILayout.Width(Screen.width/3), 
		                      GUILayout.Height(Screen.height/15))) {
			if (!string.IsNullOrEmpty (userName.Trim ())) {
				userName = userName;
			}
		}

		GUILayout.EndHorizontal ();

		scrollPosition = GUILayout.BeginScrollView (scrollPosition, true, false, 
		                                            GUILayout.Width (Screen.width/3),
			                                            GUILayout.Height (Screen.height / 2));
		for (int i = chatHistory.Count - 1; i >= 0; i--)
			GUILayout.Label (chatHistory [i]);

		GUILayout.EndScrollView ();
	}

	[RPC]
	public void ChatMessage (string message)
	{
		chatHistory.Add (message);
	}

}