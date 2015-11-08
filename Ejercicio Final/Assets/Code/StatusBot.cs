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
	Action _action;                          // Estado actual de la acción en la que se encuentra el objeto
	public int iMaxEnergy;                   // Energía máxima posible del objeto
	int _iEnergy;                            // Energía actual del objeto
	public int iHitEnergyCollision = 5;      // Energía que resta al jugador al chocar contra él
	GameObject _objKiller;                   // Objeto usuario que mata a este objeto bot.
	GameObject _objKamikaze;                 // Objeto usuario que convierte en kamikaze a este objeto bot.
	
	const float fMaxTimeDeath = 2.0f;        // Tiempo máximo que el objeto permanece en estado Death
	float fTimeDeath;                        // Tiempo transcurrido en estado Death.
	
	bool bShowEnergy;                        // Flag para saber si ha que mostrar la energía o no.
	bool bNewShowEnergy;                     // Flag para saber si ya se estaba mostrando la energía o no.
	const float fMaxTimeShowEnergy = 2.0f;   // Tiempo máximo o total de mostrar la energía.
	float fTimeShowEnergy;                   // Tiempo transcurrido mostrando la energía.
	UIStateSprite barEnergy;                 // Barra de energía del bot
	float fMagnitude;                        // Magnitud de la extensión del mesh de este enemigo. 
	                                         //  Usado para situar la barra de energía.
	AudioSource thisAudioSource;
	GameObject thisGameObject;
	MotionBot motionBot;
	Transform thisTransform;
	Renderer thisRenderer;
	Stage stage;                             // Componente stage del nivel en el que nos encontramos
	ScreenInterface screenInterface;
	
	
	
	public int iEnergy
	{
		get {return _iEnergy;}
	}


	
	public Action action
	{
		get {return _action;}
	}
	
	
	
	public GameObject objKamikaze
	{
		get {return _objKamikaze;}
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
	 * MEJORAS: En realidad objUser debería contener el objeto que causó el  *
	 *   impacto (bomba, bala, etc) y no el objeto que causó el daño (a no   *
	 *   ser que el daño sea causado por un Raycast). Dentro del objeto      *
	 *   recibido se encuentra la información del usuario que generó dicho   *
	 *   objeto. Para saber si se ha recibido el objeto que causó el daño o  *
	 *   el usuario que generó el daño (porque no hay objeto generado), se   *
	 *   hace la comparación con Tag.                                        *
	 *************************************************************************/
	public void HitObject( GameObject objUser, int iSnap)
	{
		// Si el objeto no está muerto se gestiona el impacto.
		if (_action != Action.death)
		{
			bShowEnergy = true;   // Se activa el flag para que se muestre la energía.
			
			// Se disminuye la energía
			_iEnergy = Mathf.Max( _iEnergy - iSnap, 0);
			thisAudioSource.Play();
			
			// Si se acaba la energía del objeto se comprueba su estado
			if (_iEnergy == 0)
			{
				if (_action == Action.normal)
				{
					_action = Action.kamikaze;
					_objKamikaze = objUser;      // Se asigna el objeto usuario que provocó este cambio de estado en el bot
					_iEnergy = iMaxEnergy / 3;   // Se le otorga energía de reserva
					thisRenderer.material.color = Color.red;
				}
				else if (_action == Action.kamikaze)
				{
					// CREAR ANIMACION DE LA MUERTE
					fTimeDeath = 0;
					_action = Action.death;
					_objKiller = objUser;      // Se asigna el objeto usuario que mató a este bot
					thisRenderer.material.color = Color.black;
				}
			
			}
		}
	}

	
	
	/*************************************************************************
	 * FUNCION: public void ShowEnergy()                                     *
	 * DESCRIPCION: Muestra la barra de energía encima del bot.              *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno.                                                     *
	 *************************************************************************/
	public void ShowEnergy()
	{
		// Se comprueba si ha pasado el tiempo de mostrarse la energía
		fTimeShowEnergy += Time.deltaTime;
		if (fTimeShowEnergy > fMaxTimeShowEnergy)
		{
			barEnergy.hidden = true;
			bShowEnergy = false;
			bNewShowEnergy = true;
			fTimeShowEnergy = 0;
		}
		else
		{
				// Si la energía no se estaba mostrando anteriormente se activa.
			if (bNewShowEnergy)
			{
				bNewShowEnergy = false;
				barEnergy.hidden = false;
			}
			
			// Posición en coordenadas de pantalla del bot.
			Vector3 vPosRightScreen = Camera.mainCamera.WorldToScreenPoint( thisRenderer.bounds.center + 
				                            Camera.mainCamera.transform.up * fMagnitude +
				                            Camera.mainCamera.transform.right * fMagnitude);
			Vector3 vPosLeftScreen = Camera.mainCamera.WorldToScreenPoint( thisRenderer.bounds.center +
				                            Camera.mainCamera.transform.up * fMagnitude -
				                            Camera.mainCamera.transform.right * fMagnitude);
		
			// Se obtiene el ancho y alto de la barra de energía.
			int iWidth = (int)(vPosRightScreen.x - vPosLeftScreen.x);
			int iHeight = iWidth / 6;
			barEnergy.setSize( (int)(iWidth * (_iEnergy * 1.0f / iMaxEnergy)), iHeight);
			
			// La funciones de UIToolkit tienen un error de margen al situar el elemento en pantalla, 
			//  por eso se resta 5, pero no sé por qué tiene ese error.
			barEnergy.pixelsFromBottomLeft( (int)vPosLeftScreen.y, (int)(vPosLeftScreen.x - 5));
		}		
	}
	
	
	
	
	// Use this for initialization
	void Awake () 
	{
		// Se inicializa el estado del objeto.
		_iEnergy = iMaxEnergy;     
		_action = Action.normal;   
		_objKiller = _objKamikaze = null;
				
		// Se crea la barra de energía y se inicializan los parámetros relacionados.		
		barEnergy = UIStateSprite.create( "Energy.png", 0, 0);
		barEnergy.zIndex = 1;
		bShowEnergy = false;
		bNewShowEnergy = true;
		fTimeShowEnergy = 0;
		barEnergy.hidden = true;		
		
		thisAudioSource = audio;
		thisGameObject = gameObject;
		thisTransform = transform;
		thisRenderer = renderer;
		motionBot = GetComponent<MotionBot>();
		fMagnitude = thisRenderer.bounds.extents.magnitude; // Valor de la extensión del mesh de este enemigo.			
		
		// Componente Stage del objeto que representa el nivel.
		stage = GameObject.Find( "Floor").GetComponent<Stage>();
		screenInterface = GameObject.Find( "Floor").GetComponent<ScreenInterface>();
		
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		if (bShowEnergy)
			ShowEnergy();		

		if (_action == Action.death)
		{
			// Pasado el tiempo máximo de estar muerto se destruye el enemigo
			fTimeDeath += Time.deltaTime;
			if (fTimeDeath > fMaxTimeDeath)
			{
				stage.RemoveEnemy( gameObject);
				// Al destruirse el enemigo se incrementa el número de bots matados en el usuario que lo mató.
				_objKiller.SendMessage( "AddEnemiesKilled", 1);  
				Destroy( thisGameObject);
			}
		}
	
	}
	
	
	
	void OnCollisionEnter( Collision infoCollision)
	{
		Collider objCollider = infoCollision.collider;
				
		// Si choca con el jugador
		if (objCollider.CompareTag( "Player"))
		{
			StatusUser statusUser = objCollider.GetComponent<StatusUser>();
			statusUser.HitObject( thisGameObject, iHitEnergyCollision);
		}
		else
		{
			// Si choca con cualquier otro elemento mientras está en estado normal
			//  se modifica el movimiento para que deje de chocar.
			if (_action == Action.normal)
			{
				motionBot.bNewMotion = true;
				SendMessage( "ConfigAutoDigitalMotion");
			}
		}
	}
	
	
}
