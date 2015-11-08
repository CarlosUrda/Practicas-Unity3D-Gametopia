using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage : MonoBehaviour 
{
	List <GameObject> listEnemies;         // Lista de enemigos existentes actualmente en la partida
	public GameObject[] aPrefabEnemies;    // Prefabs de enemigos posibles en el juego.
	public Vector3[] aPosStartEnemies;     // Posiciones de salida posibles de los enemigos.
	
	public enum Winner {user, cpu, draw, none};  // Posibles ganadores de la partida.
	public Winner winner;                        // Estado que indica el ganador de la partida
	public int iTargetEnemiesKilled = 10;        // Objetivo de enemigos a matar para acabar la partida.
	
	int iMaxTimeCreate = 10;       // Máximo tiempo posible a transcurrir entre la creación de enemigos
	int iTimeCreate;               // Tiempo transcurrido desde la creación del último enemigo.
	
	
	
	/*************************************************************************
	 * FUNCION: void CreateEnemy()                                           *
	 * DESCRIPCION: Crea un nuevo objeto enemigo y lo añade a la lista de    *
	 *   enemigos existentes actualmente. El nuevo enemigo creado parte de   *
	 *   un prefab y una posición elegida de manera aleatoria entre las      *
	 *   posibles predefinidas.                                              *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno                                                    *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	void CreateEnemy()
	{
		GameObject tmpEnemy = Instantiate( aPrefabEnemies[Random.Range( 0, aPrefabEnemies.Length)], 
			                               aPosStartEnemies[Random.Range( 0, aPosStartEnemies.Length)],
			                               Quaternion.identity) as GameObject;
		listEnemies.Add( tmpEnemy);
	}
	

	
	/*************************************************************************
	 * FUNCION: void RemoveEnemy( GameObject objEnemy)                       *
	 * DESCRIPCION: Se elimina el objeto enemigo de la lista de enemigos     *
	 *   existentes actualmente.                                             *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: GameObject objEnemy: Objeto enemigo a eliminar.            *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void RemoveEnemy( GameObject objEnemy)
	{
		// Se elimina el enemigo de la lista de enemigos existentes
		listEnemies.Remove( objEnemy);
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		listEnemies = new List<GameObject>();
		iTimeCreate = 0;
		winner = Winner.none;
	
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
