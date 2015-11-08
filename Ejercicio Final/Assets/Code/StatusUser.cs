using UnityEngine;
using System.Collections;

public class StatusUser : MonoBehaviour 
{
	public int iMaxEnergy = 100;    // Máxima energía que dispone el usuario
	int _iEnergy;                   // Energía del usuario
	int _iEnemiesKilled;            // Número de enemigos matados

	
	
	public int iEnergy
	{
		get 
		{ 
			return _iEnergy;
		}
	}

	
	
	public int iEnemiesKilled
	{
		get 
		{ 
			return _iEnemiesKilled;
		}
	}

	
	
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
		_iEnemiesKilled += iAddKilled;
		
	}
	
	
	
	/*************************************************************************
	 * FUNCION: public void HitObject()                                      *
	 * DESCRIPCION: Función encargada de modificar el objeto del jugador en  *
	 *   caso de recibir un impacto.                                         *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: GameObject objUser: Objeto que provoca el impacto.         *
	 *            int iSnap: Cantidad de energía a quitar del objeto.        *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void HitObject( GameObject objUser, int iSnap)
	{
		// Se hace la comparación por si en un futuro hacemos al jugador inmune a ciertos enemigos
		if (objUser.CompareTag( "Enemy"))
			_iEnergy = Mathf.Max( _iEnergy - iSnap, 0);
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		// Se inicializan las variables
		_iEnergy = iMaxEnergy;
		_iEnemiesKilled = 0;

	}

	
	
	// Update is called once per frame
	void Update () 
	{				
	}	
}
