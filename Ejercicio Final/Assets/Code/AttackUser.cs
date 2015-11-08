using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// AUMENTAR LA VELOCIDAD DEL KAMIKAZE UNA VEZ ENTRA EL BOT EN ESE ESTADO.
// ANIMACIONES, SONIDO Y EXPLOSIONES
// INTERFAZ, 
// PUNTO DE MIRA EN PRIMERA PERSONA
// CREAR BOTON PARA TIPO DE CAMARA
// MOSTRAR ENERGIA DEL JUGADOR, DE LOS ENEMIGOS AL DARLOS, LA ENERGÍA DEL CARGADOR, NUMERO DE ENEMIGOS VIVOS,
// NUMERO DE ENEMIGOS MATADOS, TECLAS PULSADAS
// REVISAR LAS ASIGNACIONES PUBLICAS DE REFERENCIAS, COLOCANDO DIRECTAMENTE EL COMPONENTE Y NO EL OBJETO.
// EVITAR QUE LA PARABOLA APAREZCA POR DEBJO DEL ESCENARIO
// MOSTRAR EL TIEMPO QUE TARDA EN SALIR UN ENEMIGO
// MOSTRAR EL TIEMPO QUE TARDA EN EXPLOTAR LA BOMBA.
// MOSTRAR ALGUNA SEÑAL DE QUE ESTA EN MODO KAMIKAZE EL ENEMIGO.
// SIMULAR EL EFECTO DEL IMPACTO DE LAS BALAS Y LAS BOMBAS EN LOS ENEMIGOS
// SOLUCIONAR EL TEMBLEQUE DE LA PISTOLA EN PRIMERA PERSONA
// SOLUCIONAR EL SALTITO AL LANZAR LA GRANADA

// NUMERO DE ENEMIGOS EN PANTALLA, NUMERO DE ENEMIGOS MUERTOS, ENERGÍA DE JUGADOR, FLECHAS DE DIRECCION, 
//  MIRILLA, BOTON DE TIPO DE CAMARA, ENERGIA DEL CARGADOR

// HACER TODO FUNCIONES GET

/*************************************************************************
 * CLASE: public class Attack : MonoBehaviour                            *
 * DESCRIPCION: Clase encargada de manejar el disparo de balas desde el  *
 *   objeto que es manejado por el usuario.                              *
 * ***********************************************************************/
public class AttackUser : MonoBehaviour 
{
	int iMaxLevelBomb = 100;             // Máxima energía que puede alcanzar el lanzamiento de la bomba
	int iSnapLevelBomb = 2;              // Salto en el incremento de la energía de disparo de la bomba
	int iLevelBomb;                      // Energía del disparo actual
	Vector3 vDirBomb;                    // Dirección de tiro de las bombas
	public int iMaxVelocityBomb = 10;    // Máxima velocidad con la que pueden salir las bombas disparadas.   
	Vector3 _vVelocityBomb;              // Velocidad inicial con que salen las bombas disparadas.
	public float fAngleBomb = 45.0f;     // Ángulo de inclinación para que el disparo de las bombas no salga plano
	                                     // sino que salga haciendo una parábola. 
		  
	public int iMaxLevelShot = 100;      // Máxima energía que puede alcanzar el cargador de disparo antes de saturarse.
	public int iSnapLevelShot = 1;       // Incremento en la barra de energía de disparo del cargador.
	int _iLevelShot;                     // Nivel actual del cargador de disparo.
	public float fDistanceShot = 100.0f; // Distancia de alcance del disparo.
	public int iHitEnergyShot = 1;       // Energía a restar en el objeto impactado al disparar.
	
	public Rigidbody prefabBomb;         // Prefab de la bomba a partir de la cual se crearán 
	                                     // el resto de bombas a lanzar
	public Renderer prefabHit;           // Prefab del impacto de la bala.
	
	List <Rigidbody> listBombs;          // Lista de bombas existentes sin explotar en la partida	

	RaycastHit hit;                      // Información del último raycast realizado al disparar.
	public LayerMask layerShot;          // Máscara que se ve afectada por los tiros.
	
	Transform thisTransform;             // Referencia al componente Transform del objeto
	Transform gun;                       // Referencia a Gun desde el cual se disparará
	Transform grenades;                  // Referencia a Grenades desde donde se lanzarán las granadas.
	GameObject thisGameObject;           // Referencia al componente GameObject del objeto
	SimulateFire simulateFire;           // Referencia al componente SimulateFire;
	
	
	
	public Vector3 vVelocityBomb
	{
		get	{return _vVelocityBomb;}
	}

	
	
	public int iLevelShot
	{
		get	{return _iLevelShot;}
	}

	
	
	/*************************************************************************
	 * FUNCION: void Shot()                                                  *
	 * DESCRIPCION: Disparo de una bala.                                     *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	void Shot()
	{		
		// Se lanza el raycast para comprobar si impacta sobre un objeto. Se lanza a partir de la distancia
		// de la pistola ya que no se puede disparar por dentrás de la pistola, sólo por delante.
		if (Physics.Raycast( thisTransform.position + thisTransform.forward * gun.renderer.bounds.extents.z, 
			                 thisTransform.forward, out hit, fDistanceShot, layerShot))
		{
			// Se usa el tag en lugar de las capas en el Raycast debido a que como queremos que la
			// simulación de los disparos no traspase ningún objeto, sea enemigo o no, el raycast
			// debe afectar a todos los objetos para obtener la distancia a ellos y así realizar
			// la simulación crrectamente. Además, evitamos que los disparos traspasen los objetos.
			if (hit.collider.CompareTag( "Enemy"))
			{
				// Se quita energía en el objeto alcanzado.
				StatusBot statusBot = hit.collider.GetComponent<StatusBot>();
				statusBot.HitObject( thisGameObject, iHitEnergyShot);
				
				// Se deja una marca en el objeto impactado
				Vector3 angle = Quaternion.LookRotation( hit.normal).eulerAngles;
				angle = new Vector3( angle.x + 90, angle.y, angle.z);
				Quaternion newRotation = Quaternion.Euler( angle);
				Renderer tmpHit = Instantiate( prefabHit, hit.point, newRotation) as Renderer;			
				
				// La marca del impacto se asocia como objeto hijo al objeto impactado como padre
				tmpHit.transform.parent = hit.transform;
			}
			// Se fija la información del objeto donde impactó la bala para realizar la simulación.
			simulateFire.hitShot = hit;
		}
		else
		{
			// Distancia máxima que alcanza la simulación de tiro al no dar ningún objeto
			simulateFire.hitShot = new RaycastHit();
			simulateFire.hitShot.point = thisTransform.position + thisTransform.forward * fDistanceShot;
			simulateFire.hitShot.distance = fDistanceShot;
		}
		
		// La pistola apunta al objetio que estamos disparando.
		gun.LookAt( hit.point);
	}
	
	
	
	/*************************************************************************
	 * FUNCION: void ThrowBomb()                                             *
	 * DESCRIPCION: Lanzamiento de una bomba                                 *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	void ThrowBomb()
	{
		// Se crea el objeto bomba a lanzar.
		Rigidbody tmpBomb = Instantiate( prefabBomb, grenades.position, grenades.rotation) as Rigidbody;
		tmpBomb.GetComponent<Bomb>().objUser = gameObject; // En el objeto bomba se fija el objeto usuario que creó dicha bomba
		listBombs.Add( tmpBomb);                           // Se añade la bomba a la lista de bombas existentes.
		
		// Se lanza la bomba con la última velocidad de la simulación que depende de la energía empleada en el lanzamiento.
		tmpBomb.AddForce( _vVelocityBomb, ForceMode.VelocityChange);		                                                 
	}
	
	
	
	/*************************************************************************
	 * FUNCION: void RemoveBomb( Rigidbody objBomb)                          *
	 * DESCRIPCION: Elimina el objeto bomba de la lista de bombas existentes *
	 *   actualmente.                                                        *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: GameObject objBomb: Objeto bomba a destruir.               *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void RemoveBomb( Rigidbody objBomb)
	{
		// Se elimina la bomba de la lista de bombas existentes
		listBombs.Remove( objBomb);
	}
	
	
	
	// Use this for initialization
	void Awake () 
	{		
		// Se obtienen los componentes iniciales.
		thisTransform = transform;
		thisGameObject = gameObject;
		gun = thisTransform.FindChild( "Gun");		
		grenades = thisTransform.FindChild( "Grenades");		
		simulateFire = GetComponent<SimulateFire>();
		
		// Se crea la lista donde estarán las bombas existentes
		listBombs = new List<Rigidbody>();

		// Se inicializan variables.
		simulateFire.bSimulateBomb = false;
		iLevelBomb = 0;
		_iLevelShot = 0;
	
	}

	
	
	// Update is called once per frame
	void Update() 
	{		
		// Se procesa el botón de disparo de balas
		if (Input.GetButton( "Fire1"))
		{
			_iLevelShot = Mathf.Min( _iLevelShot + iSnapLevelShot, iMaxLevelShot);

			// Si la energía del cargador no se ha llenado se puede disparar.
			if (_iLevelShot < iMaxLevelShot)
			{
				Shot();				
				simulateFire.bSimulateShot = true;    // Al disparar se simulan las balas
			}
			else
				simulateFire.bSimulateShot = false;   // Si se llena el cargador se deja de dispara
			
		}
		else
		{
			// Si el botón de disparo no se pulsa, baja la energía del cargador el doble de rápido.
			_iLevelShot = Mathf.Max( _iLevelShot - iSnapLevelShot * 2, 0);
			gun.forward = thisTransform.forward;   // Si no se dispara se restaura la posición de la pistola.
			simulateFire.bSimulateShot = false;    // Al dejar de disparar se dejan de simular las balas
		}

		// Se procesa el botón de lanzamiento de granadas		
		if (Input.GetButtonUp( "Fire2"))  // Si el botón de lanzar bombas se suelta, se tira la bomba
		{
			simulateFire.bSimulateBomb = false;    // Al lanzar la bomba se deja de simular el lanzamiento
			ThrowBomb();
			iLevelBomb = 0;
		}
		else
		{
			// Si se pulsa el botón de lanzar bombas aumenta la energía de lanzamiento.
			if (Input.GetButton( "Fire2"))
			{
				iLevelBomb = Mathf.Min( iLevelBomb + iSnapLevelBomb, iMaxLevelBomb);
				// Se obtiene la fuerza de lanzamiento en función del nivel de energía.
				
				// Se asigna la dirección de tiro que se usará para lanzar las bombas.
				vDirBomb = Quaternion.AngleAxis( -fAngleBomb, grenades.right) * grenades.forward;
		
				// Se calcula la velocidad a la que saldrá la bola en este punto de energía de lanzamiento.
				_vVelocityBomb = vDirBomb * (iMaxVelocityBomb * (iLevelBomb * 1.0f / iMaxLevelBomb));
				simulateFire.bSimulateBomb = true;
			}
		}
			
	}
}