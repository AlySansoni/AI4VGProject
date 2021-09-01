using System.Collections;
using UnityEngine;

[RequireComponent (typeof (Terrain))]

public class PerlinWalk : MonoBehaviour {

	public Texture2D baseHeightMap = null;
	public int resolution = 50;
	public bool makeItFlat = false;
	public float amplitude = 0f;
	public bool addHarmonics = false;
	public int randomSeed = 0;

	private Terrain t;

	void Start () {
		if (randomSeed == 0) randomSeed = (int) System.DateTime.Now.Ticks;

		t = GetComponent<Terrain> ();

		StartCoroutine(GenerateLandscape());
	}

	private void OnValidate() {
		if (t)
			StartCoroutine(GenerateLandscape());
	}

	private IEnumerator GenerateLandscape() {

		yield return new WaitForEndOfFrame();

		int lx = t.terrainData.heightmapResolution;
		int ly = t.terrainData.heightmapResolution;

		if (makeItFlat) {
			t.terrainData.SetHeights (0, 0, new float [ly, lx]);
			yield break;
		}

		Random.InitState (randomSeed);

		float [,] height = new float [ly, lx];
		
		if (baseHeightMap) {
			int tx = baseHeightMap.width;
			int ty = baseHeightMap.height;
			for (int i = 0 ; i < lx ; i += 1) {
				for (int j = 0; j < ly; j += 1) {
					int rx = (int) Mathf.Lerp(0f, (float)tx, ((float) i / lx));
					int ry = (int) Mathf.Lerp(0f, (float)ty, ((float) j / ly));
					height[j, i] = baseHeightMap.GetPixel(rx, ry).grayscale;
				}
			}
		}
		height = Amplify(height, lx, ly, 0.75f);
		
		float [,] noise = CreateNoise(lx, ly, resolution);
		noise = Amplify(noise, lx, ly, amplitude);
		for (int i = 0 ; i < lx ; i += 1) {
			for (int j = 0; j < ly; j += 1) {
				height[j, i] += noise[j, i];
			}
		}

		if (addHarmonics) {
			float [,] noise2 = Amplify(CreateNoise(lx, ly, resolution / 2), lx, ly, amplitude / 2);
			float [,] noise4 = Amplify(CreateNoise(lx, ly, resolution / 4), lx, ly, amplitude / 4);
			float [,] noise8 = Amplify(CreateNoise(lx, ly, resolution / 8), lx, ly, amplitude / 8);
			for (int i = 0 ; i < lx ; i += 1) {
				for (int j = 0; j < ly; j += 1) {
					height[j, i] += noise2[j, i] + noise4[j, i] + noise8[i, j];
				}
			}
			height = ToGround(height, lx, ly);
		}

		t.terrainData.SetHeights (0, 0, height);
		yield return new WaitForEndOfFrame();
	}

	private float[,] CreateNoise(int x, int y, int resolution) {

		int xOut = 1 + resolution * (1 + (int)Mathf.Ceil (x / resolution));
		int yOut = 1 + resolution * (1 + (int)Mathf.Ceil (y / resolution));

		float [,] h = new float [y, x];
		Vector2[][] slopes = new Vector2[yOut][];
		for (int index = 0; index < yOut; index++)
		{
			slopes[index] = new Vector2[xOut];
		}

		// first, set up the slopes and height at lattice points
		for (int i = 0; i < xOut; i += resolution) {
			for (int j = 0; j < yOut; j += resolution) {
				slopes[j][i] = Random.insideUnitCircle;
				//	h [j, i] = 0;
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {

				// find the neighbouring lattice points
				int floorI = resolution * ((int) Mathf.Floor ((float) i / resolution));
				int floorJ = resolution * ((int) Mathf.Floor ((float) j / resolution));
				int ceilI  = resolution * ((int) Mathf.Ceil  ((float) i / resolution));
				int ceilJ  = resolution * ((int) Mathf.Ceil  ((float) j / resolution));

				// calculate the four contribution to height 
				float h1 = Vector2.Dot (slopes[floorJ][floorI], new Vector2 (i, j) - new Vector2 (floorI, floorJ));
				float h2 = Vector2.Dot (slopes[floorJ][ceilI], new Vector2 (i, j) - new Vector2 (ceilI, floorJ));
				float h3 = Vector2.Dot (slopes[ceilJ][floorI], new Vector2 (i, j) - new Vector2 (floorI, ceilJ));
				float h4 = Vector2.Dot (slopes[ceilJ][ceilI], new Vector2 (i, j) - new Vector2 (ceilI, ceilJ));

				// calculate relative position inside the square
				float u = ((float) i - floorI) / resolution;
				float v = ((float) j - floorJ) / resolution;

				// interpolate by lerping first horizontally (for each couple) and then vertically
				float l1 = Mathf.Lerp (h1, h2, Slope(u));
				float l2 = Mathf.Lerp (h3, h4, Slope(u));
				float finalH = Mathf.Lerp (l1, l2, Slope(v));

				h [j, i] = finalH;
			}
		}
		return Normalize(h, x, y);
	}

	private static float Slope (float x) {
		return -2f * Mathf.Pow (x, 3) + 3f * Mathf.Pow (x, 2);
	}

	private float [,] Normalize (float [,] m, int x, int y) {
		float max, min;
		max = float.MinValue;
		min = float.MaxValue;
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				if (m [j, i] < min) min = m [j, i];
				if (m [j, i] > max) max = m [j, i];
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] = (m [j, i] - min) / (max - min);
			}
		}
		return m;
	}

	private float [,] Amplify (float [,] m, int x, int y, float amplitude) {
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] *= amplitude;
			}
		}
		return m;
	}

	private float [,] ToGround (float [,] m, int x, int y) {
		float min = float.MaxValue;
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				if (m [j, i] < min) min = m [j, i];
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] -= min;
			}
		}
		return m;
	}
	

}