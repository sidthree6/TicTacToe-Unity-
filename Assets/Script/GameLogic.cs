// Name: Siddharth Panchal (100455462)
// Purpose: This is a simple tictactoe game logic where it calculates computer movement using min-max algorithm

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

	// Get the camera object, so we can raycast to get selected object through camera
	public Camera camera;

	public GameObject pX; //= Instantiate(Resources.Load("playerX")) as GameObject;

	public GameObject pO; //= Instantiate(Resources.Load("playerO")) as GameObject;

	// 0 means no one has selected that box, 1 means box represents player X and 2 means box represents player O
	public enum BoxVal { None, PlayerX, PlayerO };

	// Box with the size of 9 to store values from users.
	private int[] Box = new int[9];

	// Array with position for each box space
	private float[,] BoxPos = new float[9,2] { {-4,4} , {0,4} , {4,4} , {-4,0} , {0,0} , {4,0} , {-4,-4} , {0,-4} , {4,-4} };

	// All winning combo possible in array to check if someone won the game or the game is a draw
	private int[,] WinCombo = new int[8,3] { {0,1,2} , {3,4,5} , {6,7,8}, {0,3,6} , {1,4,7} , {2,5,8} , {0,4,8} , {2,4,6} };

	// Whose turn is it? 0 means player X and 1 means player O
	private int curTurn = 0;

	// Is it AI Turn?
	private bool aiTurn = false;

	private bool buttonClicked = false;

	//Which box is hit?
	private int storeHit = 10;

	// True means MAX and false means MIN
	private bool minOrMax = false; 

	// Winner 0 = none, 1 = X , 2 = O
	private int winner = 0;

	// 1 means human, 2 means computer
	private int first = 0;

	private bool chosen = false;

	private bool pTaken = false;

	// Use this for initialization
	void Start () {
		// Initialize array and fill whole array with 0;
		for(int i=0;i<Box.Length;i++)
		{
			Box[i] = (int)BoxVal.None;
		}
	}

	// This function will be used by minmax algorithm
	int checkforwin()
	{
		for(int i=0;i<WinCombo.GetLength(0);i++)
		{
			// If X has any winnning combo, return 1
			if(Box[WinCombo[i,0]] == 1 && Box[WinCombo[i,1]] == 1 && Box[WinCombo[i,2]] == 1)
				return 1;
			// If O has any winnning combo, return 1
			else if(Box[WinCombo[i,0]] == -1 && Box[WinCombo[i,1]] == -1 && Box[WinCombo[i,2]] == -1)
				return -1;
		}
		// else return 0;
		return 0;
	}

	// This function will take player and movement in account
	bool checkWin(int place, bool who, bool check)
	{
		int p=0;
		int inc=0;
		if(who == true)
			p = 1;
		else
			p = -1;

		// Place X or O on movement according to which play chose it.
		Box[place] = p;

		// Goes through all of all winning combination
		for(int i=0;i<WinCombo.GetLength(0);i++)
		{
			if(Box[WinCombo[i,0]] == p && Box[WinCombo[i,1]] == p && Box[WinCombo[i,2]] == p)
				inc++;
		}

		if(check == true)
			Box[place] = 0;

		if(inc == 0)
		{
			return false;
		}
		else
		{
			return true; // Player won
		}
	}

	// Check if board is draw.
	bool checkDraw()
	{
		for(int i=0;i<Box.Length;i++)
		{
			if(Box[i] == 0)
				return false;
		}

		return true;
	}

	// Coroutine function which will be called when its computers turn
	IEnumerator aiMove(bool max)
	{
		int best = -2;
				
		int where = -1;
		int p=0;
		if(max == true)
			p = 1;
		else
			p = -1;
		
		//create list to store all best vales
		List<int> list = new List<int>();
		List<int> place = new List<int>();
		
		for(int i=0;i<Box.Length;i++)
		{
			if(Box[i] == 0)
			{
				Box[i] = p;
				int val = -minmax(p*-1);
				Box[i] = 0;
				if(val >= best)
				{
					best  = val;
					where = i;
					
					list.Add(best);
					place.Add(where);
				}
				
			}
			
		}
		
		if (checkDraw() == true)
		{
			Debug.Log("Draw");
		}
		else
		{
			bool run = true;
			
			//Debug.Log(where+" "+best);

			// Goes through all best values and randomize the movement form them.
			while(run)
			{
				int rand = Random.Range(0,list.Count);
				
				if(list[rand] == best)
				{
					//Debug.Log(rand+" "+place[rand]);
					where = place[rand];
					run = false;
				}
			}
			
			if(first == 1)
			{
				Instantiate(pO, new Vector3(BoxPos[where,0], 0.0f , BoxPos[where,1]), Quaternion.identity);
				pTaken = false;
				if(checkWin(where,false,false))
				{
					winner = 2;
					Debug.Log("Computer(O) Wins!");
				}
			}
			else
			{
				Instantiate(pX, new Vector3(BoxPos[where,0], 0.0f , BoxPos[where,1]), Quaternion.identity);
				pTaken = false;
				if(checkWin(where,false,false))
				{
					winner = 2;
					Debug.Log("Computer(X) Wins!");
				}
			}
		}
		yield return null;
		
	}

	//Min-Max tree algorithm , Max means X and Min means O
	/*int minmax(int player)
	{
		int ifwin = checkforwin();
		if(ifwin != 0) 
			return ifwin*player;
		if (checkDraw () == true)
			return 0; 

		int best = -2;

		for(int i=0;i<Box.Length;i++)
		{
			if(Box[i] == 0)
			{
				Box[i] = player;

				int val = -minmax(player*-1);
								
				Box[i] = 0;
				if(val > best)
					best = val;
			}

		}
		return best;
	}*/

	// Min-Max tree algorithm
	int minmax(int player)
	{
		//First check if player has already won or game is draw. if that the case return 1,-1 or 0
		int ifwin = checkforwin();
		if(ifwin != 0) 
			return ifwin;
		if (checkDraw () == true)
			return 0; 
		
		int best1 = -2;
		int best2 = 2;
		int best = 0;

		// Goes throgh loop
		for(int i=0;i<Box.Length;i++)
		{
			// Only check if cell is empty
			if(Box[i] == 0)
			{
				Box[i] = player; // Tries to move a player at current cell
				
				int val = minmax(player*-1); // Do recusrion with alternate player
				
				Box[i] = 0; // Revert the cell back to 0

				if(player == 1)
				{
					// If its X turn, do MAX
					if(val > best1)
						best1 = val;
				}
				else
				{
					// If its O turn, do MIN
					if(val < best2)
						best2 = val;
				}
			}
			
		}
		if(player == -1)
			best = best2;
		else
			best = best1;

		// Return best move
		return best;
	}

	// GUI to display HUD
	void OnGUI()
	{
		if(chosen == true)
		{
			string move="";
			string cmove="";

			if(first == 1)
			{
				move = "X";
				cmove = "O";
			}
			else if(first == 2)
			{
				move = "O";
				cmove = "X";
			}

			GUI.Box(new Rect(10,10,100,25), "You Are '"+move+"'");

			GUI.Box(new Rect(10,50,120,25), "Computer is '"+cmove+"'");

			if(winner == 1)
			{
				GUI.Box(new Rect(Screen.width/2 - 300, 50, 600, 600), "YOU WON!");
			}
			if(winner == 2)
			{
				GUI.Box(new Rect(Screen.width/2 - 300, 50, 600, 600), "COMPUTER WON!");
			}
			if (checkDraw() == true && winner == 0)
			{
				GUI.Box(new Rect(Screen.width/2 - 300, 50, 600, 600), "DRAW!");
			}

			if(pTaken == true)
			{
				GUI.Box(new Rect(10,100,300,25), "Place is Already Taken! Please choose other cell.");
			}

			GUI.Box (new Rect (10,150,100,100), "Menu");
			
			// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
			if (GUI.Button (new Rect (20,190,80,20), "Restart")) {
				Application.LoadLevel("one");
			}

			// Make the second button.
			if (GUI.Button (new Rect (20,220,80,20), "Quit")) {
				Application.Quit();
			}
		}
	}

	// Update is called once per frame
	void Update () {

		// If who goes first isnt chosen display menu
		if(chosen == false)
		{
			if (Input.GetMouseButtonDown(0))
			{ 
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)){
					
					string hitS = hit.collider.gameObject.name;
					// If user hits computer ICON. Ai goes first
					if(hitS == "PC")
					{
						first = 2; // first is set to AI
						aiTurn = true;
						chosen = true;

						// Destroy menu items
						GameObject menuBG = GameObject.Find("Plane");
						Destroy(menuBG);
						GameObject light = GameObject.Find("light");
						Destroy(light);
						GameObject PC = GameObject.Find("PC");
						PC.GetComponent<Renderer>().enabled=false;
						Destroy(PC);
						GameObject whogoesfirst = GameObject.Find("whoGoesFirst");
						whogoesfirst.GetComponent<Renderer>().enabled=false;
						Destroy(whogoesfirst);
						GameObject human = GameObject.Find("Human");
						human.GetComponent<Renderer>().enabled=false;
						Destroy(human);
					}
					// If user hits Human ICON. We go first
					if(hitS == "Human")
					{
						first = 1; // first is set to Human
						aiTurn = false;
						chosen = true;

						// Destroy menu items
						GameObject menuBG = GameObject.Find("Plane");
						Destroy(menuBG);
						GameObject light = GameObject.Find("light");
						Destroy(light);
						GameObject PC = GameObject.Find("PC");
						Destroy(PC);
						GameObject whogoesfirst = GameObject.Find("whoGoesFirst");
						Destroy(whogoesfirst);
						GameObject human = GameObject.Find("Human");
						Destroy(human);
					}
				}
			}
		}
		else
		{
			//if Winner isnt chosen.
			if(winner == 0)
			{
				if(aiTurn == true)
				{
					// If its ai turn, run minmax algorithm
					StartCoroutine(aiMove(false));
					aiTurn=false;
					curTurn = 0;
				}
				else
				{
					// If left button pressed
					if (Input.GetMouseButtonDown(0))
					{ 
						Ray ray = camera.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit)){

							string hitS = hit.collider.gameObject.name;
							if(hitS == "0" || hitS == "1" || hitS == "2" || hitS == "3" || hitS == "4" || hitS == "5" || hitS == "6" || hitS == "7" || hitS == "8")
							{
								storeHit = int.Parse(hit.collider.gameObject.name);
								buttonClicked = true;
							}
						}
					}
				}
				// If butten is clicked and legal move, place human selected item in cell
				if(buttonClicked == true)
				{
					if(Box[storeHit] == 0)
					{
							if(first == 1)
							{
								Instantiate(pX, new Vector3(BoxPos[storeHit,0], 0.0f , BoxPos[storeHit,1]), Quaternion.identity);
								pTaken = false;
								if(checkWin(storeHit,true,false))
								{
									winner = 1;
									Debug.Log("You(X) Wins!");
								}
								aiTurn = true;
							}
							else
							{
								Instantiate(pO, new Vector3(BoxPos[storeHit,0], 0.0f , BoxPos[storeHit,1]), Quaternion.identity);
								pTaken = false;
								if(checkWin(storeHit,true,false))
								{
									winner = 1;
									Debug.Log("You(O) Wins!");
								}
								aiTurn = true;
							}

					}
					else
					{
						pTaken = true;
					}

					buttonClicked = false;
				}

			}
		
		}

	}
}
