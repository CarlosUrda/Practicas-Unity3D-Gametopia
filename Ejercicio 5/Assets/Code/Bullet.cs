using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	float fMaxTime = 5.0f;    // Tiempo máximo de vida de la bala en segundos
	float fTime = 0.0f;       // Tiempo de vida de la bala.

	// Use this for initialization
	void Start () 
	{
		fTime = 0.0f;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Pasado el tiempo máximo de vida se destruye la bala.
		fTime += Time.deltaTime;
		if (fTime > fMaxTime)
			Destroy( gameObject);
	
	}
}
