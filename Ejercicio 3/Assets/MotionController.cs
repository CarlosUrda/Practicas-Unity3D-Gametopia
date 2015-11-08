using UnityEngine;
using System.Collections;


/*************************************************************************
 * CLASE: public class MotionController : MonoBehaviour                  *
 * DESCRIPCION: Controlador del movimiento de un objeto que consiste en  *
 *   realizar el movimiento del objeto de manera aleatoria hacia atrás o *
 *   hacia adelante durante 5 segundos o un tiempo aleatorio entre 1 y 5 *
 *   segundos, según se configure. El objeto no podrá moverse más allá   *
 *   de unos límites puestos en coordenadas del mundo global.            *
 * ***********************************************************************/
public class MotionController : MonoBehaviour
{
	public int iMotionTime = 5;        // Tiempo máximo que va a durar cada movimiento
	public bool bRandomTime = false;   // Flag para activar tiempo de duración aleatorio en cada movimiento
	public float fSpeed;               // Velocidad del movimiento	
	public float fMaxGlobalX = 10.0f;  // Límites del movimiento del objeto controlado en coordenadas 
	public float fMinGlobalX = -10.0f; // globales adelante, atrás, izquierda y derecha
	public float fMaxGlobalZ = 10.0f;  
	public float fMinGlobalZ = -10.0f;
	public enum Motion {idle = 0, forward, back};  // Tipos posibles de movimientos
	
	Motion currentMotion;              // Movimiento que se está ejecutando actualmente
	float fStartTime;                  // Momento de tiempo en que se inició el movimiento
	int iCurrentMotionTime;            // Tiempo que va a durar el movimiento actual.
	Transform thisTransform;           // Referencia al componente Transform del objeto

	
	
	/*************************************************************************
	 * FUNCION: public Motion getCurrentMotion()                             *
	 * DESCRIPCION: Función encargada de obtener el tipo de movimiento que   *
	 *   el objeto está realizando actualmente.                              *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Motion: Tipo de movimiento que está realizando el objeto.    *
	 *************************************************************************/
	public Motion GetCurrentMotion()
	{
		return currentMotion;
	}
	
	
	
	/*************************************************************************
	 * FUNCION: public Motion spentMotionTime()                              *
	 * DESCRIPCION: Función que obtiene el tiempo transcurrido en segundos   *
	 *   desde que se inicio el movimiento actual.                           *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: float: Segundos transcurridos por el movimiento actual.      *
	 *************************************************************************/
	public float SpentMotionTime()
	{
		return (Time.timeSinceLevelLoad - fStartTime);
	}
	
	
	
	/*************************************************************************
	 * FUNCION: void configMotion()                                          *
	 * DESCRIPCION: Función encargada de generar el nuevo tipo de movimiento *
	 *   que va a realizar el objeto en la siguiente acción, junto con el    *
	 *   tiempo que tardará en realizar dicho movimiento.                    *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno (void)                                               *
	 *************************************************************************/
	void ConfigMotion()
	{
		// Se obtiene el tiempo en segundos que va a durar el nuevo movimiento a realizar
		iCurrentMotionTime = (bRandomTime) ? Random.Range( 1, iMotionTime+1) : iMotionTime;
		
		fStartTime = Time.timeSinceLevelLoad;  // Momento del tiempo en que se inicia el nuevo movimiento.
		
		// Se obtiene el nuevo movimiento a realizar
		currentMotion = (Motion) Random.Range( (int) Motion.forward, (int) Motion.back + 1);
	}
	
	
	
	/*************************************************************************
	 * FUNCION: void swap<T>( ref T val1, ref T val2)                        *
	 * DESCRIPCION: Función encargada de intercambiar los valores de dos     *
	 *   variables de tipo genérico.                                         *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: var1, var2: Variables a intercambiar sus valores.          *
	 *   SALIDA: var1, var2: Variables con los valores ya intercambiados     *
	 * RETORNO: Ninguno (void)                                               *
	 *************************************************************************/
	void swap<T>( ref T var1, ref T var2)
	{
		T varTemp = var1;
		var1 = var2;
		var2 = varTemp;
	}
	
	
	
	// Use this for initialization
	void Start ()
	{
		// Se comprueba que los valores máximo y mínimo de los límites están en las variables correctas.
		if (fMaxGlobalX < fMinGlobalX)
			swap<float>( ref fMaxGlobalX, ref fMinGlobalX);
		if (fMaxGlobalZ < fMinGlobalZ)
			swap<float>( ref fMaxGlobalZ, ref fMinGlobalZ);
					
		thisTransform = transform;   // Se obtiene la referencia al componente Transform
		
		// Se comprueba que la posición del objeto está dentro de los límites, y en caso de no estarlo
		// se sitúa al objeto dentro de los límites.
		thisTransform.position = new Vector3( Mathf.Clamp( thisTransform.position.x, fMinGlobalX, fMaxGlobalX), thisTransform.position.y,
			                                  Mathf.Clamp( thisTransform.position.z, fMinGlobalZ, fMaxGlobalZ));

		ConfigMotion();              // Se configura el primer movimiento del objeto
		
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
		// Si el movimiento ha superado el tiempo de duración se configura un nuevo movimiento
		if (SpentMotionTime() > iCurrentMotionTime)
		{
			ConfigMotion();
		}
				
		// Se realiza el movimiento actualmente configurado.
		switch (currentMotion)
		{
		case Motion.forward:
			// Se obtiene el vector de desplazamiento (en coordenadas globales del mundo) a realizar durante este fotograma
			Vector3 globalForward = thisTransform.forward * fSpeed * Time.deltaTime;

			// Se comprueba si el desplazamiento sobrepasa los límites globales impuestos. El objeto no se podrá
			// desplazar más allá de los límites.
			globalForward = new Vector3( Mathf.Clamp( globalForward.x, fMinGlobalX - thisTransform.position.x, 
				                                      fMaxGlobalX - thisTransform.position.x), 
				                         globalForward.y,
				                         Mathf.Clamp( globalForward.z, fMinGlobalZ - thisTransform.position.z, 
				                                      fMaxGlobalZ - thisTransform.position.z));
			
			// Se realiza el desplazamiento relativo a las coordenadas del mundo global
			thisTransform.Translate( globalForward, Space.World);
			break;
			
		case Motion.back:
			Vector3 globalBack = thisTransform.forward * -fSpeed * Time.deltaTime;

			globalBack = new Vector3( Mathf.Clamp( globalBack.x, fMinGlobalX - thisTransform.position.x, 
				                                   fMaxGlobalX - thisTransform.position.x), 
				                      globalBack.y,
				                      Mathf.Clamp( globalBack.z, fMinGlobalZ - thisTransform.position.z, 
				                                   fMaxGlobalZ - thisTransform.position.z));

			thisTransform.Translate( globalBack, Space.World);
			break;
			
		case Motion.idle:
		default:
			break;
		}
			
	}
}

