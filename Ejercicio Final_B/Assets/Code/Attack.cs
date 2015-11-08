using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// AÑADIR LA EXTENSIÓN DE BOUNDS AL LANZAR LA BOLA, Y EN LA CAMARA Y EN LA PARÁBOLA
// 

/*************************************************************************
 * CLASE: public class Attack : MonoBehaviour                            *
 * DESCRIPCION: Clase encargada de manejar el disparo de balas desde el  *
 *   objeto que es manejado por el usuario.                              *
 * ***********************************************************************/
public class Attack : MonoBehaviour 
{
	public int iMaxLevelBomb = 100;      // Máxima energía que puede alcanzar el lanzamiento de la bomba
	int iSnapLevelBomb = 1;              // Salto en el incremento de la energía de disparo de la bomba
	int iLevelBomb;                      // Energía del disparo actual
	Vector3 vDirBomb;                    // Dirección de tiro de las bombas
	public int iMaxForceBomb = 1000;     // Máxima velocidad con la que salen las bombas disparadas.   
	Vector3 vForceBomb;                  // Fuerza con que salen las bombas disparadas.
	public float fAngleBomb = 45.0f;     // Ángulo de inclinación para que el disparo de las bombas no salga plano
	                                     // sino que salga haciendo una parábola. 
		  
	public int iMaxLevelShot = 100;      // Máxima energía que puede alcanzar el cargador de disparo antes de saturarse.
	const int iSnapLevelShot = 5;        // Incremento en la energía de disparo.
	int iLevelShot;                      // Nivel actual del cargador de disparo.
	const float fDistanceShot = 1000.0f; // Distancia de alcance del disparo.
	const int iHitEnergyShot = 5;        // Energía a restar en el objeto impactado al disparar.
	
	public Rigidbody prefabBullet;       // Prefab de la bala a partir de la cual se crearán 
	                                     // el resto de proyectiles a disparar.
	public Renderer prefabHit;           // Prefab del impacto de la bala.
	
	public LayerMask layerTarget;        // Capa de los objetos que afectarán a los disparos.
	List <Rigidbody> listBombs;          // Lista de bombas existentes en la partida	

	Transform thisTransform;             // Referencia al componente Transform del objeto
	GameObject thisGameObject;           // Referencia al componente GameObject del objeto
	SimulateFire simulateFire;           // Referencia al componente SimulateFire;
	
	
	
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
		RaycastHit hit;
		
		// Se lanza el raycast para comprobar si impacta sobre un objeto
		if (Physics.Raycast( thisTransform.position, thisTransform.forward, out hit, fDistanceShot, layerTarget))
		{
			// Se quita energía en el objeto alcanzado.
			hit.collider.SendMessage( "HitObject", thisGameObject, iHitEnergyShot);

			// Se deja una marca en el objeto impactado
			Vector3 angle = Quaternion.LookRotation( hit.normal).eulerAngles;
			angle = new Vector3( angle.x + 90, angle.y, angle.z);
			Quaternion newRotation = Quaternion.Euler( angle);
            Renderer tmpHit = Instantiate( prefabHit, hit.point, newRotation);			
			
			// El impacto se asocia como objeto hijo al objeto impactado como padre
			tmpHit.transform.parent = hit.transform;
			
			// Distancia máxima que alcanza la simulación de tiro al dar un objeto
			simulateFire.fDistanceSimShot = hit.distance;   

		}
		else
			// Distancia máxima que alcanza la simulación de tiro al no dar ningún objeto
			simulateFire.fDistanceSimShot = fDistanceShot;  
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
		Rigidbody tmpBomb = Instantiate( prefabBomb, thisTransform.position, thisTransform.rotation) as Rigidbody;
		tmpBomb.SendMessage( "SetObjUser", gameObject);  // En el objeto bomba se fija el objeto usuario que creó dicha bomba
		listBombs.Add( tmpBomb);                         // Se añade la bomba a la lista de bombas existentes.
		tmpBomb.AddForce( simulateFire.vForceBomb);      // Se lanza la bomba con la última fuerza de la simulación que 
		                                                 //  depende de la energía empleada en el lanzamiento.
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
	void Start () 
	{		
		// Se obtienen los componentes iniciales.
		thisTransform = transform;
		thisGameObject = gameObject;
		simulateFire = GetComponent<SimulateFire>();
		
		// Se asigna la dirección de tiro que se usará para lanzar las bombas.
		vDirBomb = Quaternion.AngleAxis( -fAngleBomb, thisTransform.right) * thisTransform.forward;
		
		// Se crea la lista donde estarán las bombas existentes
		listBombs = new List<Rigidbody>();

		// Se inicializan variables.
		simulateFire.bSimulateBomb = false;
		iLevelBomb = 0;
		iLevelShot = 0;
	
	}

	
	
	// Update is called once per frame
	void Update() 
	{		
		// Se procesa el botón de disparo de balas
		if (Input.GetButton( "Fire1"))
		{
			iLevelShot = Mathf.Max( iLevelShot + iSnapLevelShot, iMaxLevelShot);

			// Si la energía del cargador no se ha llenado se puede disparar.
			if (iLevelShot < iMaxLevelShot)
			{
				Shot();				
				simulateFire.bSimulateShot = true;    // Al disparar se simulan las balas
			}
			else
				simulateFire.bSimulateShot = false;    // Si se llena el cargador se deja de disparar
			
		}
		else
		{
			// Si el botón de disparo no se pulsa, baja la energía del cargador.
			iLevelShot = Mathf.Max( iLevelShot - iSnapLevelShot, 0);
			simulateFire.bSimulateShot = false;      // Al dejar de disparar se dejan de simular las balas
			
			// Si el botón de lanzar bombas se suelta, se tira la bomba
			if (Input.GetButtonUp( "Fire2"))
			{
				ThrowBomb();
				simulateFire.bSimulateBomb = false;    // Al lanzar la bomba se deja de simular el lanzamiento
				iLevelBomb = 0;
			}
			else
			{
				// Si se pulsa el botón de lanzar bombas aumenta la energía de lanzamiento.
				if (Input.GetButton( "Fire2"))
				{
					iLevelBomb = Mathf.Min( iLevelBomb + iSnapLevelBomb, iMaxLevelBomb);
					// Se obtiene la fuerza de lanzamiento en función del nivel de energía.
					simulateFire.vForceBomb = vDirBomb * (iMaxForceBomb * (iLevelBomb * 1.0f / iMaxLevelBomb));
					simulateFire.bSimulateBomb = true;
				}
			}
		}
			
	}
