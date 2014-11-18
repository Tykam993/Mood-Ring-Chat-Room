//This code was written following Brent Farris' "Networking 2: Introduction" tutorial on Youtube.com
//https://www.youtube.com/watch?v=TgFyJK9gits&list=UUStD9dM0fAPxyJqrWWwFHPQ&index=21
using UnityEngine;
using System.Collections;

public class NetworkMenu : MonoBehaviour {

	public string connectionIP = "127.0.0.1";
	public int portNumber = 8632;
	
	public static bool Connected { get; private set;}


	private void OnConnectedToServer() {
		//The Server has initialized
		Connected = true;
	}

	private void OnServerInitialized() {
		//A client has just connected
		Connected = true;
	}

	private void DisconnectedFromServer() {
		//Connection Lost or disconnected
		Connected = false;
	}

	private void OnGUI() {

		if (!Connected) {
			connectionIP = GUILayout.TextField (connectionIP);
			int.TryParse(GUILayout.TextField (portNumber.ToString()), out portNumber);

			if (GUILayout.Button ("Connect"))
				Network.Connect (connectionIP, portNumber);

			if (GUILayout.Button ("Host"))
				Network.InitializeServer (4, portNumber, true);
		} 
		else {
			GUILayout.Label ("Connections: " + Network.connections.Length.ToString ());
		}
	}
}
