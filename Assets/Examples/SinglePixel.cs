using UnityEngine;
using System.Collections;

public class SinglePixel : MonoBehaviour {

	Fupixel fupixel;

	// Use this for initialization
	void Start () {
		fupixel = gameObject.GetComponent<Fupixel>();
		fupixel.ClearPixels(Color.black);
		fupixel.SetPixel(10, 10, Color.red);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
