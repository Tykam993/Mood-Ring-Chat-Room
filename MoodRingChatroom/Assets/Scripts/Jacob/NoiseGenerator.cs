using UnityEngine;
using System.Collections;

public class NoiseGenerator : MonoBehaviour {

	// The width and height of the plane
	private int planeWidth;
	private int planeHeight;

	// Variable used to scale the coordinates
	private float scale = 1.0F;

	// Adds randomness to the colored noise
	private float rand = 5.0F;
	private float change = 1f;

	// The texture
	private Texture2D noiseTex;

	// An array of colors that will map to the entire plane
	private Color[] colorNoise;

	void Start() {
		// Set the width and height to the plane's dimensions
		planeWidth = (int) gameObject.renderer.bounds.size.x;
		planeHeight = (int) gameObject.renderer.bounds.size.y;

		// Creates a new textures with the current dimensions
		noiseTex = new Texture2D(planeWidth, planeHeight);

		colorNoise = new Color[noiseTex.width * noiseTex.height];
		gameObject.renderer.material.mainTexture = noiseTex;
	}

	void Update() {
		// MAKE. SOME. NOISE.
		GeneratePerlin();

		// 
		rand = rand+0.05f*change;
		if(rand > 20f || rand < 5f) {
			print("switch");
			change*=-1f;;
		}
	}

	void GeneratePerlin() {
		for(float x = 0.0F; x < noiseTex.width; x++) {
			for(float y = 0.0F; y < noiseTex.height; y++) {
				// Scales the coordinates 
				float xCoord = x / noiseTex.width * scale;
				float yCoord = y / noiseTex.height * scale;

				// Calculates the colored noise
				float perlinColor = Mathf.PerlinNoise(xCoord/rand, yCoord*rand);

				// Sets the current pixel to the colored noise
				colorNoise[(int) y * noiseTex.width + (int) x] = new Color(perlinColor, perlinColor, perlinColor);
			}
		}
		noiseTex.SetPixels(colorNoise);
		noiseTex.Apply();
	}
}