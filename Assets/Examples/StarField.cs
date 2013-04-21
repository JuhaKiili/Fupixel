using UnityEngine;
using System.Collections;

public class StarField : MonoBehaviour {

	Fupixel fupixel;
	Vector3[] stars;

	public int count = 1000;
	public float minZ = 1f;
	public float maxZ = 50f;
	public float xSize = 512f;
	public float ySize = 512f;
	public float speed = 50f;
	
	void Awake () {
		fupixel = gameObject.GetComponent<Fupixel>();
		fupixel.ClearPixels(Color.black);
		stars = new Vector3[count];

		for(int i=0; i<stars.Length; i++)
			stars[i] = new Vector3(Random.Range(-xSize, xSize), Random.Range(-ySize, ySize), (float)(stars.Length - i) * maxZ / stars.Length + minZ);
	}

	void Update() 
	{
		fupixel.ClearPixels(Color.black);

		for (int i = 0; i < stars.Length; i++)
		{
			int x = (int)(stars[i].x / stars[i].z) + fupixel.width / 2;
			int y = (int)(stars[i].y / stars[i].z) + fupixel.height / 2;
			byte c = (byte)(255f / stars[i].z);
			
			if (x < fupixel.width)
				stars[i] += new Vector3(Time.deltaTime * speed, 0f, 0f);
			else
				stars[i] = new Vector3(-xSize, stars[i].y, stars[i].z);

			if(x >= 0 && x < fupixel.width && y >= 0 && y < fupixel.height)
				fupixel.SetPixel(x, y, c, c, c, 255);
		}
	}
}
