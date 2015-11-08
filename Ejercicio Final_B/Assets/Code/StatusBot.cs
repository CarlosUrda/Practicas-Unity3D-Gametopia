using UnityEngine;
using System.Collections;

/*************************************************************************
 * CLASE: public class StatusBot : MonoBehaviour                         *
 * DESCRIPCION: Clase encargada de gestionar el estado de energía del    *
 *   objeto manejado por la máquina.                                     *
 * ***********************************************************************/

[RequireComponent(typeof(AudioSource))]
public class StatusBot : MonoBehaviour 
{
	public enum Action {normal, kamikaze, death};   // Tipos de estado de acciones en las que se puede encontrar el objeto.
	public Action action;                           // Estado actual de la acción en la que se encuentra el objeto
	public int iMaxEnergy = 100;             // Energía máxima posible del objeto
	public const int iSnapEnergy = 5;        // Salto de energía por defecto que se resta al recibir un impacto.
//	public GameObject objCam;                // Objeto cámara del juego.
//	Interface intfce;                        // Componente script Interface de la cámara del juego
	public GameObject objStage;              // Objeto Stage del juego (objeto que representa el nivel general actual)
	Stage stage;                             // Componente stage del nivel en el que nos encontramos
	GameObject objKiller;                    // Objeto usuario que mata a este objeto bot.
	GameObject objKamikaze;                  // Objeto usuario que convierte en kamikaze a este objet obot.
	
	int iEnergy;                       // Energía actual del objeto
	const float fMaxTimeDeath = 5.0f;  // Tiempo máximo que el objeto permanece en estado Death
	int fTimeDeath;                    // Tiempo transcurrido en estado Death.
	
//	Animation thisAnimation;
	AudioSource thisAudioSource;
	GameObject thisGameObject;
	
	
	
	/*************************************************************************
	 * FUNCION: public int GetEnergy()                                       *
	 * DESCRIPCION: Obtener la energía actual del objeto.                    *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: int: Energía del objeto                                      *
	 *************************************************************************/
	public int GetEnergy()
	{
		return iEnergy;
	}

	
	
	/*************************************************************************
	 * FUNCION: public void HitObject()                                      *
	 * DESCRIPCION: Función encargada de modificar el objeto bot en caso de  *
	 *   recibir un impacto.                                                 *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: GameObject objUser: Objeto usuario que provoca el impacto. *
	 *            int iSnap: Cantidad de energía a quitar del objeto.        *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void HitObject( GameObject objUser, int iSnap = iSnapEnergy)
	{
		// Si el objeto no está muerto se gestiona el impacto.
		if (action != Action.death)
		{
			// Se disminuye la energía
			iEnergy = Mathf.Max( iEnergy - iSnap, 0);
			thisAudioSource.Play();
			
			// Si se acaba la energía del objeto se comprueba su estado
			if (iEnergy == 0)
			{
				if (action == Action.normal)
				{
					action = Action.kamikaze;
					objKamikaze = objUser;      // Se asigna el objeto usuario que provocó este cambio de estado en el bot
					iEnergy = iMaxEnergy / 4;   // Se le otorga energía de reserva
				}
				else if (action == Action.kamikaze)
				{
					// CREAR ANIMACION DE LA MUERTE
					fTimeDeath = 0.0f;
					action = Action.death;
					objKiller = objUser;      // Se asigna el objeto usuario que mató a este bot
				}
			
			}
		}
	}

	
	
	// Use this for initialization
	void Awake () 
	{
		// Se inicializa el estado del objeto.
		iEnergy = iMaxEnergy;     
		action = Action.normal;   
		
//		thisAnimation = animation;	
		thisAudioSource = audio;
		thisGameObject = gameObject;
		
//		intfce = objCam.GetComponent<Interface>(); 	// Componente script Interface de la cámara del juego.
		stage = objStage.GetComponent<Stage>();     // Componente Stage del objeto stage que representa el nivel.
		objKiller = objKamikaze = null;
		
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		if (action == Action.death)
		{
			// Pasado el tiempo máximo de estar muerto se destruye el enemigo
			fTimeDeath += Time.deltaTime;
			if (fTimeDeath > fMaxTimeDeath)
			{
				stage.RemoveEnemy( gameObject);
				// Al destruirse el enemigo se incrementa el número de bots matados en el usuario que lo mató.
				objKiller.SendMessage( "AddEnemiesKilled");  
				Destroy( thisGameObject);
			}
		}
	
	}
	
	
}
