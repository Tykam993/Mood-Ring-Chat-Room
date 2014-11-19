﻿using UnityEngine;
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
	private float flux = 0.05f;

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

		// Compensates for active/deactive states of emotion
		if(EmotionModel.CurrentState.y < 0) {
			flux = 0.01f;
		}
		else if(EmotionModel.CurrentState.y >= 0) {
			flux = 0.05f;
		}

		// Changes the random factor
		rand = rand+flux*change;
		if(rand > 20f || rand < 5f) {
			change*=-1f;;
		}
	}

	void GeneratePerlin() {
		float r = 1.0f-EmotionModel.CurrentState.x/-1.0f;
		float g = 1.0f-EmotionModel.CurrentState.magnitude;
		float b = 1.0f-EmotionModel.CurrentState.x/1.0f;

		for(float x = 0.0F; x < noiseTex.width; x++) {
			for(float y = 0.0F; y < noiseTex.height; y++) {
				// Scales the coordinates 
				float xCoord = x / noiseTex.width * scale;
				float yCoord = y / noiseTex.height * scale;

				// Calculates the colored noise
				float perlinColor = Mathf.PerlinNoise(xCoord/rand, yCoord*rand);

				// Sets the current pixel to the colored noise
				colorNoise[(int) y * noiseTex.width + (int) x] = new Color(perlinColor * r, perlinColor * g, perlinColor * b);
			}
		}
		noiseTex.SetPixels(colorNoise);
		noiseTex.Apply();
	}
}