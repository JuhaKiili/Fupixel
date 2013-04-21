using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Fupixel : MonoBehaviour 
{
	public int width = 320;
	public int height = 240;

	[HideInInspector]
	public Texture2D texture;
	[HideInInspector]
	public Color32[] pixels;

	private Color32[] clearCache;
	private Color32 clearCacheColor;

	public void SetPixel(float x, float y, Color color)
	{
		pixels[(int)y * width + (int)x] = (Color32)color;
	}

	public void SetPixel(int x, int y, Color color)
	{
		pixels[y * width + x] = (Color32)color;
	}

	public void SetPixel(int x, int y, Color32 color)
	{
		pixels[y * width + x] = color;
	}

	public void SetPixel(int x, int y, float r, float g, float b)
	{
		pixels[y * width + x] = new Color32((byte)(r * 255f), (byte)(r * 255f), (byte)(r * 255f), 255);
	}

	public void SetPixel(int x, int y, float r, float g, float b, float a)
	{
		pixels[y * width + x] = new Color32((byte)(r * 255f), (byte)(r * 255f), (byte)(r * 255f), (byte)(a * 255f));
	}

	public void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
	{
		pixels[y * width + x] = new Color32(r, g, b, a);
	}

	public void SetPixel(int index, Color color)
	{
		pixels[index] = (Color32)color;
	}

	public void SetPixel(int index, Color32 color)
	{
		pixels[index] = color;
	}

	public void SetPixel(int index, float r, float g, float b)
	{
		pixels[index] = new Color32((byte)(r * 255f), (byte)(r * 255f), (byte)(r * 255f), 255);
	}

	public void SetPixel(int index, float r, float g, float b, float a)
	{
		pixels[index] = new Color32((byte)(r * 255f), (byte)(r * 255f), (byte)(r * 255f), (byte)(a * 255f));
	}

	public void SetPixel(int index, byte r, byte g, byte b, byte a)
	{
		pixels[index] = new Color32(r, g, b, a);
	}

	public Color32 GetPixel32(int x, int y)
	{
		return pixels[y * width + x];
	}

	public Color GetPixel(int x, int y)
	{
		return (Color)pixels[y * width + x];
	}

	public Color32 GetPixel32(int index)
	{
		return pixels[index];
	}

	public Color GetPixel(int index)
	{
		return (Color)pixels[index];
	}

	public int GetIndex(int x, int y)
	{
		return y * width + x;
	}

	public void Awake()
	{
		if (!Application.isPlaying)
			ApplySettings();
	}

	public void ClearPixels(Color color)
	{
		if (color != clearCacheColor || clearCache.Length != pixels.Length)
		{
			int len = pixels.Length;
			
			if (clearCache == null || clearCache.Length != len)
				clearCache = new Color32[len];

			for (int i = 0; i < len; i++)
				clearCache[i] = color;

			clearCacheColor = color;
		}
		clearCache.CopyTo(pixels, 0);
	}

	public void LateUpdate()
	{
		texture.SetPixels32(pixels);
		texture.Apply();
	}
	
	public void ApplySettings()
	{
		if (width < 4 || height < 4)
			return;

		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
			meshFilter = gameObject.AddComponent<MeshFilter>();

		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer == null)
			meshRenderer = gameObject.AddComponent<MeshRenderer>();

		Mesh mesh = meshFilter.sharedMesh;

		if (mesh == null)
			mesh = new Mesh();

		Vector3[] verts = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		int[] triangles = new int[6];

		int u = 0;
		uv[u++] = new Vector2(0f, 0f);
		uv[u++] = new Vector2(1f, 0f);
		uv[u++] = new Vector2(1f, 1f);
		uv[u++] = new Vector2(0f, 1f);

		int v = 0;
		verts[v++] = new Vector2(0f, 0f);
		verts[v++] = new Vector2(1f, 0f);
		verts[v++] = new Vector2(1f, 1f);
		verts[v++] = new Vector2(0f, 1f);

		int t = 0;
		triangles[t++] = 0;
		triangles[t++] = 1;
		triangles[t++] = 2;
		triangles[t++] = 0;
		triangles[t++] = 2;
		triangles[t++] = 3;

		mesh.vertices = verts;
		mesh.uv = uv;
		mesh.triangles = triangles;

		meshFilter.sharedMesh = mesh;
		
		if (texture != null)
			DestroyImmediate(texture);
		
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;

		pixels = new Color32[width * height];

		for (int x = 0; x < width; x++)
		{
			pixels[x] = new Color32(255, 255, 255, 255);
			pixels[width * height - x - 1] = new Color32(255, 255, 255, 255);
		}

		for (int y = 0; y < height; y++)
		{
			pixels[y * width] = new Color32(255, 255, 255, 255);
			pixels[(y + 1) * width - 1] = new Color32(255, 255, 255, 255);
		}

		texture.SetPixels32(pixels);
		texture.Apply();

		if (meshRenderer.sharedMaterials.Length == 0)
			meshRenderer.sharedMaterials = new Material[1];

		if (meshRenderer.sharedMaterials[0] == null)
		{
			Material[] materials = meshRenderer.sharedMaterials;
			materials[0] = new Material(Shader.Find("Fupixel"));
			meshRenderer.sharedMaterials = materials;
		}

		if(Camera.mainCamera != null)
			transform.position = Camera.mainCamera.transform.position + Camera.mainCamera.transform.forward;
		
		mesh.name = "Fupixel Mesh";
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(100000f, 100000f, 100000f));

		meshRenderer.sharedMaterials[0].SetTexture("_MainTex", texture);
	}
}

