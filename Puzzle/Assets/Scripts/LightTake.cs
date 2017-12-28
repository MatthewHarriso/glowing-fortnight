using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTake : MonoBehaviour
{
	bool lightHit;

	Renderer rend;
	
	// Use this for initialization
	void Start ()
	{
		lightHit = false;

		rend = GetComponent<Renderer>();
		rend.material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (lightHit)
		{
			rend.material.color = Color.cyan;
		}
		else
		{
			rend.material.color = Color.black;
		}
	}

	public void LightHit(bool l_state)
	{
		lightHit = l_state;
	}

	void OnRayCastEnter(GameObject sender)
	{
		lightHit = true;
	}

	void OnRayCastExit(GameObject sender)
	{
		lightHit = false;
	}
}
