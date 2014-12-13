//This code was written following Code Compile's "Unity3D Series - Networking Part 1" tutorial on Youtube
// https://www.youtube.com/watch?v=GEnkgqSH51E

using UnityEngine;
using System.Collections;


public class NetworkManager : MonoBehaviour {

	int GUIWidth = 350, GUIHeight = 500;
	public GameObject startServerButton;
	string registeredGameName = "ProcJam_MoodRing_Chat_Server";
	bool isRefreshing = false;
	float refreshRequestLength = 3.0f;
	HostData[] hostData;



	public void StartServer() {

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
		GUILayout.BeginVertical (GUILayout.Width (Screen.width/2));

		if (Network.isClient || Network.isServer) {

			return;
		}

		GUILayout.BeginHorizontal (GUILayout.Width (GUIWidth));
		if (GUILayout.Button("Start New Server",GUILayout.Width(Screen.width/3), 
		               GUILayout.Height(Screen.height/3))) {

			//Start server function
			StartServer();


		}
		GUILayout.EndHorizontal ();

		GUILayout.Space (Screen.height/4);

		GUILayout.BeginHorizontal (GUILayout.Width (GUIWidth));
		if(GUILayout.Button("Refresh Server List",GUILayout.Width(Screen.width/3), 
		              GUILayout.Height(Screen.height/3))) {
			//refresh server list
			StartCoroutine("RefreshHostList");
		}
		GUILayout.EndHorizontal ();
		if (hostData != null) {

			for(int i = 0; i < hostData.Length ; i++) {
				if(GUI.Button(new Rect(Screen.width/2, 65f + (30f * i), Screen.width/2, 
				                       Screen.height/10), hostData[i].gameName)) {

					Network.Connect(hostData[i]);
				}
			}
		}
		GUILayout.EndVertical ();
	}
}