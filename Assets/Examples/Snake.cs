using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Snake : MonoBehaviour {

	public Color snakeColor = Color.cyan;
	public Color backgroundColor = Color.black;
	public Color wallColor = Color.blue;
	public Color foodColor = Color.yellow;

	Fupixel fupixel;
	int snakeX;
	int snakeY;
	Queue<int> snakeParts;
	int deltaX=0;
	int deltaY=1;
	int frameCount;

	int foodIndex;
	public bool inputHandled = true;
	
	void Awake () 
	{
		fupixel = gameObject.GetComponent<Fupixel>();
		fupixel.ClearPixels(Color.black);
		snakeParts = new Queue<int>();
		snakeX = fupixel.width / 2;
		snakeY = fupixel.height / 4;
		foodIndex = fupixel.GetIndex(snakeX, fupixel.height / 2);
		snakeParts.Enqueue(fupixel.GetIndex(snakeX, snakeY));
		DrawWalls();
	}
	
	void Update () 
	{
		HandleInput();

		// Make it run half the speed
		if (frameCount % 2 == 0)
		{
			MoveSnake();
			CheckForGameOver();
			DrawFood();
			DrawSnake();
			HandleSnakeQueue();
		}
		
		frameCount++;
	}

	private void CheckForGameOver()
	{
		if (snakeX < 1 || snakeX >= fupixel.width - 1 || snakeY < 1 || snakeY >= fupixel.height - 1 || fupixel.pixels[fupixel.GetIndex(snakeX, snakeY)] == snakeColor)
		{
			Application.LoadLevel(0);
		}
	}

	private void HandleSnakeQueue()
	{
		snakeParts.Enqueue(fupixel.GetIndex(snakeX, snakeY));
		if (fupixel.GetIndex(snakeX, snakeY) != foodIndex)
		{
			snakeParts.Dequeue();
		}
		else
		{
			foodIndex = fupixel.GetIndex(Random.Range(1, fupixel.width - 2), Random.Range(1, fupixel.height - 2));
		}
	}

	private void DrawSnake()
	{
		fupixel.SetPixel(snakeX, snakeY, snakeColor);
		fupixel.SetPixel(snakeParts.Peek(), backgroundColor);
	}

	private void MoveSnake()
	{
		snakeX += deltaX;
		snakeY += deltaY;
		inputHandled = true;
	}

	private void HandleInput()
	{
		if (!inputHandled)
			return;

		if (Input.GetKeyDown(KeyCode.LeftArrow) && deltaX == 0)
		{
			deltaX = -1;
			deltaY = 0;
			inputHandled = false;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) && deltaX == 0)
		{
			deltaX = 1;
			deltaY = 0;
			inputHandled = false;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && deltaY == 0)
		{
			deltaX = 0;
			deltaY = 1;
			inputHandled = false;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) && deltaY == 0)
		{
			deltaX = 0;
			deltaY = -1;
			inputHandled = false;
		}
	}

	void DrawFood()
	{
		fupixel.SetPixel(foodIndex, foodColor);
	}

	void DrawWalls()
	{
		for (int x = 0; x < fupixel.width; x++)
		{
			fupixel.SetPixel(x, wallColor);
			fupixel.SetPixel(fupixel.width * fupixel.height - x - 1, wallColor);
		}

		for (int y = 0; y < fupixel.height; y++)
		{
			fupixel.SetPixel(y * fupixel.width, wallColor);
			fupixel.SetPixel((y + 1) * fupixel.width - 1, wallColor);
		}
	}
}
