﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameController : MonoBehaviour {

	public Text text;
	public GameObject picture;
	public Button[] buttonList;


	public GameObject check;
	public GameObject wrong;

	public GameObject lives;

	//Hangman 
	/*
	public GameObject head;
	public GameObject body;
	public GameObject leftArm;
	public GameObject rightArm;
	public GameObject rightLeg;
	public GameObject leftLeg;*/

	public string solution = "solucao";
	private int lettersNumber;
	public int lostLives = 0;
	Minigame1Status status;

	public void Start()
	{
		if (!System.IO.File.Exists(Application.persistentDataPath + "/cenas"))
		{
			status = new Minigame1Status ();
		} else 
		{
			status = FileManager.ReadFromBinaryFile<Minigame1Status> (Application.persistentDataPath + "/cenas");
		}
		
		picture.GetComponent<Image> ().sprite = Resources.Load (StaticSceneManager.GetSceneImage(), typeof(Sprite)) as Sprite;
		solution = picture.GetComponent<Image> ().sprite.name.ToUpper();
		text.text = " ";
		setBlanks (solution);
		setButtons (solution);
	}
		
	public void Update()
	{
		if (lostLives == 1)
			lives.transform.GetChild(2).gameObject.SetActive (false);
		else if (lostLives == 2)
			lives.transform.GetChild(1).gameObject.SetActive (false);
		else if (lostLives == 3) {
			lives.transform.GetChild(0).gameObject.SetActive (false);
			gameLose ();
		}

		if (lettersNumber == solution.Length) {
			lettersNumber = 0;
			gameWin ();
		}
	}

	void gameWin()
	{
		check.SetActive (true);

		for (int i = 0; i < buttonList.Length; i++) {
			buttonList [i].gameObject.SetActive (false);
		}

		status.ingredients [solution] = 1;
		FileManager.WriteToBinaryFile (Application.persistentDataPath + "/cenas", status);
	}


	void gameLose()
	{
		wrong.SetActive (true);

		for (int i = 0; i < buttonList.Length; i++) {
			buttonList [i].gameObject.SetActive (false);
		}
	}

	public void loadScene(string level)
	{
		SceneManager.LoadScene (level);
	}

	void setBlanks(string solution)
	{
		for (int i = 0; i < solution.Length; i++) {
			text.text += "_ ";
		}
	}

	void setButtons(string solution)
	{
		HashSet<char> chars = new HashSet<char> (solution);
		List<char> letters = new List<char> ();
		List<int> randomButton = new List<int> () { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

		while (chars.Count != 15) {
			int charNumber = Random.Range (65, 90);
			chars.Add ((char)charNumber);
		}
			
		foreach (char letter in chars) {
			letters.Add (letter);
		}
			
		for (int i = 0; i < letters.Count; i++) { 
			int randomNumber = Random.Range (0, randomButton.Count);
			buttonList [randomButton[randomNumber]].GetComponentInChildren<Text> ().text = letters [i].ToString();
			Button btn = buttonList [randomButton [randomNumber]];
			buttonList [randomButton [randomNumber]].onClick.AddListener(delegate{taskOnClick(btn, solution);});
			randomButton.RemoveAt (randomNumber);
		}

	}

	public void taskOnClick(Button button, string solution)
	{
		string buttonText = button.GetComponentInChildren<Text> ().text;
		button.interactable = false;
		if (solution.Contains (buttonText)) {
			List<int> foundIndexes = new List<int> ();
			for (int i = 0; i < solution.Length; i++) {
				if (solution [i].ToString () == buttonText) {
					foundIndexes.Add (i);
				}
			}

			for (int i = 0; i < foundIndexes.Count; i++) {
				lettersNumber++;
				if (foundIndexes [i] == 0) {
					text.text = " " + buttonText + text.text.Substring (2);
				} else {
					text.text = text.text.Substring (0, (foundIndexes [i] * 2) + 1) + buttonText + text.text.Substring ((foundIndexes [i] * 2) + 2); 
				}
			}
		} else
			lostLives++;
	}
}
