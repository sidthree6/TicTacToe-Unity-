using UnityEngine;
using System.Collections;

public class MouseOver : MonoBehaviour {
	public Material icon;
	public Material iconover;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseOver() {
		GetComponent<Renderer>().material = iconover;
	}
	
	void OnMouseExit() {
		GetComponent<Renderer>().material = icon;
	}
}
