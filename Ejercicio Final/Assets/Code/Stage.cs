using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*************************************************************************
 * CLASE: public class Stage : MonoBehaviour                             *
 * DESCRIPCION: Clase encargada de gestionar el nivel de maneral general *
 *   como la creación de enemigos, saber cuando se acaba la partida, etc *
 * ***********************************************************************/
public class Stage : MonoBehaviour 
{
	List <GameObject> _listEnemies;              // Lista de enemigos existentes actualmente en la partida
	public GameObject[] aPrefabEnemies;         // Prefabs de enemigos posibles en el juego.
	public Vector3[] aPosStartEnemies;          // Posiciones de salida posibles de los enemigos.
	
	public enum Winner {user, cpu, draw, none}; // Posibles ganadores de la partida.
	Winner winner;                              // Estado que indica el ganador de la partida
	public int iFinishEnemiesKilled = 10;       // Objetivo de enemigos a matar para acabar la partida.
	
	public float fMaxTimeCreate = 6.0f;         // Máximo tiempo posible a transcurrir entre la creación de enemigos
	float fTimeCreate;                          // Tiempo transcurrido desde la creación del último enemigo.
	
	StatusUser statusUser;                      // Referencia al componente StatusUser del jugador.
	
	
	
	public List<GameObject> listEnemies
	{
		get {return _listEnemies;}
	}
	
	
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
		_listEnemies.Add( tmpEnemy);
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
		_listEnemies.Remove( objEnemy);
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		_listEnemies = new List<GameObject>();
		fTimeCreate = 0.0f;
		winner = Winner.none;

		statusUser = GameObject.Find( "User").GetComponent<StatusUser>();
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		// Se comprueba si ha ocurrido algún estado para que acabe la partida.
		if (statusUser.iEnergy == 0)
		{
			winner = Winner.cpu;
			/*
			// No hay ganador por ahora, sólo a ver cuántos enemigos se matan
			if (statusUser.iEnemiesKilled == iFinishEnemiesKilled)
				winner = Winner.draw;
			else
				winner = Winner.cpu;
			*/
		}
		/*
		else if (statusUser.iEnemiesKilled == iFinishEnemiesKilled)
			winner = Winner.user;
		*/

		// Se comprueba si ha terminado la partida
		if (winner != Winner.none)
		{
			if (PlayerPrefs.GetInt( "Record", 0) < statusUser.iEnemiesKilled)
				PlayerPrefs.SetInt( "Record", statusUser.iEnemiesKilled);
			
			winner = Winner.none;
			Application.Quit();
		}
		
		// Se comprueba si se debe crear un nuevo enemigo
		fTimeCreate += Time.deltaTime;
		if (fTimeCreate > fMaxTimeCreate)
		{
			CreateEnemy();
			fTimeCreate = 0.0f;
		}
	
	}
}
