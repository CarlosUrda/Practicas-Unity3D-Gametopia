using UnityEngine;
using System.Collections;

public class StatusUser : MonoBehaviour 
{
	const int iMaxEnergy = 100;         // Máxima energía que dispone el usuario
	int iEnergy;                        // Energía del usuario
	const int iSnapCollision = 2;       // Energía a restar en caso de collisionar con un enemigo
	int iEnemiesKilled;                 // Número de enemigos matados

	public GameObject objStage;   // Objeto Stage del juego (objeto que representa el nivel general actual)
	Stage stage;                  // Componente stage del nivel en el que nos encontramos
	
	
	
	/*************************************************************************
	 * FUNCION: void AddEnemiesKilled( int iAddKilled)                       *
	 * DESCRIPCION: Incremento del número de enemigos matados.               *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: int iAddKilled: Número de enemigos matados a incrementar.  *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	void AddEnemiesKilled( int iAddKilled = 1)
	{
		iEnemiesKilled += iAddKilled;
		
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		// Se inicializan las variables
		iEnergy = iMaxEnergy;
		iEnemiesKilled = 0;

		stage = objStage.GetComponent<Stage>();     // Componente Stage del objeto stage que representa el nivel.
		
	
	}

	
	
	// Update is called once per frame
	void Update () 
	{
		// Se comprueba si ha ocurrido algún estado para que acabe la partida.
		if (iEnergy == 0)
		{
			if (iEnemiesKilled == stage.iTargetEnemiesKilled)
				stage.winner = Stage.Winner.draw;
			else
				stage.winner = Stage.Winner.cpu;
		}
		else if (iEnemiesKilled == stage.iTargetEnemiesKilled)
			stage.winner = Stage.Winner.user;
				
	}
	
	
	
	void OnCollisionEnter( Collision collisionInfo)
	{
		// Se comprueba
		if (collisionInfo.collider.CompareTag( "Enemy"))
		{
			iEnergy = Mathf.Max( iEnergy - iSnapCollision, 0);
		}
	}
}
