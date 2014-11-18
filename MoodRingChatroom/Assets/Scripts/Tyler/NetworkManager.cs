//This code was written following Code Compile's "Unity3D Series - Networking Part 1" tutorial on Youtube
// https://www.youtube.com/watch?v=GEnkgqSH51E

using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	string registeredGameName = "ProcJam_MoodRing_Chat_Server";
	bool isRefreshing = false;
	float refreshRequestLength = 3.0f;
	HostData[] hostData;

	private void StartServer() {

		Network.InitializeServer (16, 25002, false);
		MasterServer.RegisterHost (registeredGameName, "ProcJam MoodRing Chat", "Chat with mood detection");

	}

	void OnServerInitialized() {
		Debug.Log ("Server has been initialized");

	}

	void OnMasterServerEvent(MasterServerEvent masterServerEvent) {
		if (masterServerEvent == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log ("Registration successful");
		}
	}

	public IEnumerator RefreshHostList() {

		Debug.Log ("Refreshing...");
		MasterServer.RequestHostList (registeredGameName);
		float timeStarted = Time.time;
		float timeEnd = Time.time + refreshRequestLength;

		while (Time.time < timeEnd) {


			hostData = MasterServer.PollHostList ();
			yield return new WaitForEndOfFrame();
		}

		if (hostData == null || hostData.Length == 0) {

			Debug.Log ("No active servers have been found");
		} 
		else {
			Debug.Log(hostData.Length + "have been found");
		}

	}

	public void OnGUI() {

		if (Network.isClient || Network.isServer) {

			return;
		}


		if (GUI.Button(new Rect(25f,25f,150f,30f), "Start New Server")) {

			//Start server function
			StartServer();

		}

		if(GUI.Button(new Rect(25f,65f,150f,30f), "Refresh Server List")) {
			//refresh server list
			StartCoroutine("RefreshHostList");
		}

		if (hostData != null) {

			for(int i = 0; i < hostData.Length ; i++) {
				if(GUI.Button(new Rect(Screen.width/2, 65f + (30f * i), 300f, 30f), hostData[i].gameName)) {

					Network.Connect(hostData[i]);
				}
			}
		}
	}
}