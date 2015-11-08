
using UnityEngine;
using System.Collections;



/*************************************************************************
 * CLASE: public class MotionController : MonoBehaviour                  *
 * DESCRIPCION: Controlador del movimiento de un objeto que consiste en  *
 *   realizar el movimiento del objeto de manera aleatoria hacia atrás o *
 *   hacia adelante durante 5 segundos o un tiempo aleatorio entre 1 y 5 *
 *   segundos, según se configure. El objeto no podrá moverse más allá   *
 *   de unos límites puestos en coordenadas del mundo global.            *
 * MEJORAS: Cuando el movimiento se hace a través de teclado y se pulsa  *
 *   a la vez eje horizontal y vertical, ambos ejes pueden tener el      *
 *   valor 1, lo que significa que el desplazamiento en diagonal es más  *
 *   rápido que horizontal o vertical al ser el módulo del vector        *
 *   desplazamiento mayor que 1. Como la máquina simula el movimiento de *
 *   las teclas sucede esto también cuando se mueve por IA. En cambio,   *
 *   cuando el movimiento se produce a través de un stick, esto no pasa  *
 *   porque el módulo del vector desplazamiento está limitado como       *
 *   máximo al valor 1.                                                  *
 *   En realidad habría que hacer la distinción entre entrada analógica  *
 *   y estado real analógico. En este ejercicio la máquina simula una    *
 *   entrada digital y a partir de ella se modifica el estado real       *
 *   analógico, pero se podría hacer que simulase una entrada analógica  *
 *   y a partir de ella modificar el estado real analógico. Todos las    *
 *   variables que representan el movimiento analógico en el código se   *
 *   refieren al estado real analógico. Para simular entarda analógica   *
 *   habría que crear nuevas variables que guardasen dichos valores      *
 *   separándolos del estado real analógico del objeto, y a partir de    *
 *   estas variables de entrada analógica obtener el estado analógico    *
 *   real de objeto.                                                     *
 * ***********************************************************************/
public class MotionController : MonoBehaviour
{
	public int iMotionTime = 5;         // Tiempo máximo que va a durar cada movimiento
	public bool bRandomTime = false;    // Flag para activar tiempo de duración aleatorio en cada movimiento
	public float fHorSpeed;             // Velocidad del movimiento	de traslación horizontal (lateral)
	public float fVerSpeed;             // Velocidad del movimiento	de traslación vertical (adelante y atrás)
	public float fTurnSpeed;            // Velocidad del movimiento	de rotación
	public Rect limits;                 // Límites definidos por donde se puede mover el objeto
	readonly float fWidth = 20.0f;      // Valores para ancho y alto de los límites en caso 
	readonly float fHeight = 20.0f;     // de ser cero en la interfaz de unity

	// Tipos posibles de movimientos digitales del teclado simulados por IA del objeto
	public enum HorMotion {left = -1, idle, right};    // Movimientos Horizontales
	public enum VerMotion {back = -1, idle, forward};  // Movimientos Verticales
	public enum TurnMotion {left = -1, idle, right};   // Tipos posibles de giros
	HorMotion horMotion;                // Entrada digital horizontal del movimiento actual
	VerMotion verMotion;                // Entrada digital vertical del movimiento actual
	TurnMotion turnMotion;              // Entrada de giro digital ejecutándose actualmente
		
	// Movimientos analógicos en el mundo real
	float fHorMotion;                   // Valor del actual movimiento real horizontal analógico
	float fVerMotion;                   // Valor del actual movimiento real vertical analógico
	float fTurnMotion;                  // Valor del actual movimiento real de giro analógico
	readonly float fSnapHorMotion = 0.04f;      // Salto en la modificación del movimiento real horizontal analógico
	readonly float fSnapVerMotion = 0.04f;      // Salto en la modificación del movimiento real vertical analógico
	readonly float fSnapTurnMotion = 0.04f;     // Salto en la modificación del giro real analógico
	readonly float fGravityHorMotion = 0.02f;   // Gravedad del movimiento real horizontal analógico hacia el resposo
	readonly float fGravityVerMotion = 0.02f;   // Gravedad del movimiento real vertical analógico hacia el resposo
	readonly float fGravityTurnMotion = 0.02f;  // Gravedad de giro real analógico hacia el reposo

	public enum ModeMotion {auto, manual};           // Tipos de Modo de generación de movimiento. Auto = IA; Manual = Usuario
	public ModeMotion modeMotion = ModeMotion.auto;  // Modo de movimiento ejecutándose actualmente
		
	float fStartTime;                   // Momento de tiempo en que se inició el último movimiento de la máquina
	int iCurrentMotionTime;             // Tiempo que va a durar el movimiento actual de la máquina

	Transform thisTransform;            // Referencia al componente Transform del objeto
	LineRenderer thisLineRenderer;      // Referencia al componente LineRenderer para dibujar los límites.
	
	
	
	/*************************************************************************
	 * FUNCION: public void GetAnalogMotion( out float fHorMotion,           *
	 *                                       out float fVerMotion,           *
	 *                                       out float fTurnMotion)          *
	 * DESCRIPCION: Función encargada de obtener el movimiento de traslación *
	 *   y rotación analógico que el objeto está realizando actualmente.     *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: float fHorMotion: Movimiento analógico en el eje horizontal *
	 *           float fVerMotion: Movimiento analógico en el eje vertical   *
	 *           float fTurnMotion: Movimiento analógico de rotación.        *
	 * RETORNO: Ninguno.                                                     *
	 *************************************************************************/
	public void GetAnalogMotion( out float fHorMotion, out float fVerMotion, out float fTurnMotion)
	{
		fHorMotion = this.fHorMotion;
		fVerMotion = this.fVerMotion;
		fTurnMotion = this.fTurnMotion;
	}
	
	
	
	/*************************************************************************
	 * FUNCION: public void GetDigitalMotion( out HorMotion horMotion,       *
	 *                                        out VerMotion verMotion,       *
	 *                                        out TurnMotion turnMotion)     *
	 * DESCRIPCION: Función encargada de obtener el movimiento de traslación *
	 *   y rotación digital que el objeto está realizando actualmente.       *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: HorMotion horMotion: Movimiento digital en el eje horzontal *
	 *           VerMotion verMotion: Movimiento digital en el eje vertical  *
	 *           TurnMotion turnMotion: Movimiento digital de rotación.      *
	 * RETORNO: Ninguno.                                                     *
	 *************************************************************************/
	public void GetDigitalMotion( out HorMotion horMotion, out VerMotion verMotion, 
		                          out TurnMotion turnMotion)
	{
		horMotion = this.horMotion;
		verMotion = this.verMotion;
		turnMotion = this.turnMotion;
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
	 * FUNCION: void ConfigAutoDigitalMotion()                               *
	 * DESCRIPCION: Función encargada de generar el nuevo tipo de movimiento *
	 *   digital, calculado por la máquina, que va a realizar el objeto en   *
	 *   la siguiente acción, junto con el tiempo que tardará en realizar    *
	 *   dicho movimiento.                                                   *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno (void)                                               *
	 *************************************************************************/
	void ConfigAutoDigitalMotion()
	{
		// Se obtiene el tiempo en segundos que va a durar el nuevo movimiento a realizar
		iCurrentMotionTime = (bRandomTime) ? Random.Range( 1, iMotionTime+1) : iMotionTime;
		
		fStartTime = Time.timeSinceLevelLoad;  // Momento del tiempo en que se inicia el nuevo movimiento.

		// Se obtiene la nueva entrada de teclado digital como si la máquina pulsase las teclas de movimiento
		horMotion = (HorMotion) Random.Range( (int)HorMotion.left, (int)HorMotion.right + 1);
		verMotion = (VerMotion) Random.Range( (int)VerMotion.back, (int)VerMotion.forward + 1);
		turnMotion = (TurnMotion) Random.Range( (int)TurnMotion.left, (int)TurnMotion.right + 1);
	}
	


	/*************************************************************************
	 * FUNCION: void ConfigManualDigitalMotion()                             *
	 * DESCRIPCION: Función encargada de obtener el nuevo tipo de movimiento *
	 *   digital, introducido por el usuario, que va a realizar el objeto en *
	 *   la siguiente acción.                                                *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno (void)                                               *
	 *************************************************************************/
	 void ConfigManualDigitalMotion()
	 {
		// Se guarda la entrada digital horizontal del teclado pulsado por el usuario.
		if (Input.GetKey( KeyCode.LeftArrow))
		{
			if (!Input.GetKey( KeyCode.RightArrow) || (horMotion != HorMotion.right))
				horMotion = HorMotion.left;
		}
		else if (Input.GetKey( KeyCode.RightArrow))
			horMotion = HorMotion.right;
		else
			horMotion = HorMotion.idle;
			
		// Se guarda la entrada digital vertical del teclado pulsado por el usuario.
		if (Input.GetKey( KeyCode.UpArrow))
		{
			if (!Input.GetKey( KeyCode.DownArrow) || (verMotion != VerMotion.back))
				verMotion = VerMotion.forward;
		}
		else if (Input.GetKey( KeyCode.DownArrow))
			verMotion = VerMotion.back;
		else
			verMotion = VerMotion.idle;

		// Se guarda la entrada digital de giro del teclado pulsado por el usuario.
		if (Input.GetKey( KeyCode.Z))
		{
			if (!Input.GetKey( KeyCode.X) || (turnMotion != TurnMotion.right))
				turnMotion = TurnMotion.left;
		}
		else if (Input.GetKey( KeyCode.X))
			turnMotion = TurnMotion.right;
		else
			turnMotion = TurnMotion.idle;
	}



	/*************************************************************************
	 * FUNCION: void InputDigitalToRealAnalog()                              *
	 * DESCRIPCION: Función encargada de plasmar el movimiento digital de    *
	 *   entrada aplicado actualmente al objeto en el movimiento analógico a *
	 *   ser ejecutado de manera real por el mismo objeto.                   *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno (void)                                               *
	 *************************************************************************/
	void InputDigitalToRealAnalog()
	{
		// Se modifica el nuevo movimiento de entrada analógico a realizar a partir del
		// movimiento digital actual.
		if (horMotion == HorMotion.idle)
		{
			// Si no hay movimiento digital el movimiento horizontal analógico del objeto tiende a pararse hasta llegar a 0
			if (fHorMotion != 0)
				fHorMotion = Mathf.Clamp( fHorMotion - Mathf.Sign( fHorMotion) * fGravityHorMotion, 
				                          Mathf.Min( fHorMotion, 0), Mathf.Max( fHorMotion, 0));				
		}
		else  // Si hay movimiento digital, se aplica al movimiento horizontal analógico
		{
			fHorMotion = Mathf.Clamp( fHorMotion + fSnapHorMotion * (int)horMotion, -1.0f, 1.0f);
		}
			
		if (verMotion == VerMotion.idle)
		{
			// Si no hay movimiento digital el movimiento vertical analógico del objeto tiende a pararse hasta llegar a 0
			if (fVerMotion != 0)
				fVerMotion = Mathf.Clamp( fVerMotion - Mathf.Sign( fVerMotion) * fGravityVerMotion, 
				                          Mathf.Min( fVerMotion, 0), Mathf.Max( fVerMotion, 0));				
		}
		else  // Si hay movimiento digital, se aplica al movimiento vertical analógico
		{
			fVerMotion = Mathf.Clamp( fVerMotion + fSnapVerMotion * (int)verMotion, -1.0f, 1.0f);
		}
			
		if (turnMotion == TurnMotion.idle)
		{
			// Si no hay movimiento digital, el giro analógico del objeto tiende a pararse hasta llegar a 0
			if (fTurnMotion != 0)
				fTurnMotion = Mathf.Clamp( fTurnMotion - Mathf.Sign( fTurnMotion) * fGravityTurnMotion, 
				                           Mathf.Min( fTurnMotion, 0), Mathf.Max( fTurnMotion, 0));				
		}
		else  // Si hay movimiento digital, se aplica al giro analógico
		{
			fTurnMotion = Mathf.Clamp( fTurnMotion + fSnapTurnMotion * (int)turnMotion, -1.0f, 1.0f);
		}
	}
	
	
	
	// La correción de los límites se realiza en la función Awake porque la posición de la cámara depende
	// de los valores de los límites, y como dicha posición de la cámara se va a calcular una sóla vez
	// al inicio de la partida, se usa la función Awake en este script para asegurar que la corrección 
	// de los límites se realiza antes que e l cálculo de la posición de la cámara.
	// Se podría realizar el cálculo de la posición de la cámara dentro de la función update (por si los
	// límites cambian durante la ejecución), en cuyo caso no haría falta corregir los límites dentro de
	// la función Awake, con hacerlo dentro de Start sería suficiente.
	void Awake()
	{
		// Se comprueba que los valores dados como ancho y alto del límite son correctos.
		// Debido a que la estructura Rect no gestiona bien los valores negativos, se corrigen
		// los límites y la posición de manera que se puedan introducir sin problema el ancho y alto negativo.
		if (limits.width == 0) // Si el ancho es 0, se asigna el ancho por defecto 
			limits.width = fWidth;
		if (limits.width < 0)  // Si el ancho es negativo, se modifica la coordenada izquierda
		{
			limits.x += limits.width;
			limits.width = -limits.width;
		}
		
		if (limits.height == 0)
			limits.height = fHeight;
		if (limits.height < 0)  // Si el alto es negativo, se modifica la coordenada superior.
		{
			limits.y -= limits.height;
			limits.height = -limits.height;
		}
					
	}
	
	
	
	// Use this for initialization
	void Start ()
	{					
		thisTransform = transform;   // Se obtiene la referencia al componente Transform
		
		// Se comprueba que la posición del objeto está dentro de los límites, y en caso de no estarlo
		// se sitúa al objeto dentro de los límites.
		thisTransform.position = new Vector3( Mathf.Clamp( thisTransform.position.x, limits.x, limits.x + limits.width), 
			                                  thisTransform.position.y,
			                                  Mathf.Clamp( thisTransform.position.z, limits.y - limits.height, limits.y));
		
		if (modeMotion == ModeMotion.auto)    // Si el movimiento es automático se configura el primer movimiento.
			ConfigAutoDigitalMotion();        
		
		// Se configura el componente LineRenderer y se dibujan los límites en pantalla.
		thisLineRenderer = GetComponent<LineRenderer>();
		thisLineRenderer.SetVertexCount( 5);
		thisLineRenderer.SetColors( Color.black, Color.black);
		thisLineRenderer.SetWidth( 0.2f, 0.2f);
		thisLineRenderer.SetPosition( 0, new Vector3( limits.x, 0, limits.y));
		thisLineRenderer.SetPosition( 1, new Vector3( limits.x + limits.width, 0, limits.y));
		thisLineRenderer.SetPosition( 2, new Vector3( limits.x + limits.width, 0, limits.y - limits.height));
		thisLineRenderer.SetPosition( 3, new Vector3( limits.x, 0, limits.y - limits.height));					
		thisLineRenderer.SetPosition( 4, new Vector3( limits.x, 0, limits.y));				
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
		switch (modeMotion)
		{
			// Movimiento automático generado por la máquina
			case ModeMotion.auto:
				// Si el movimiento ha superado el tiempo de duración se configura un nuevo 
				// movimiento digital automático
				if (SpentMotionTime() > iCurrentMotionTime)
				{
					ConfigAutoDigitalMotion();
				}
			
				// Se convierte el movimiento digital actual a movimiento real analógico.
				InputDigitalToRealAnalog();			
				break;

			// Movimiento manual realizado por el usuario.
			case ModeMotion.manual:
				// Se obtiene la entrada digital del usuario para realizar el movimiento
				ConfigManualDigitalMotion();
			
				// Se obtienen los valores actuales del movimiento analógico real a partir de
				// los ejes proporcionados por Unity
				fHorMotion = Input.GetAxis( "Horizontal");
				fVerMotion = Input.GetAxis( "Vertical");
				fTurnMotion = Input.GetAxis( "Turn");			
				break;

			default:
				break;
		}

		// Se obtienen los vectores de desplazamiento a partir del movimiento analógico real actual
		Vector3 vTurnMotion = thisTransform.up * fTurnSpeed * Time.deltaTime * fTurnMotion;
		Vector3 vVerMotion = thisTransform.forward * fVerSpeed * Time.deltaTime * fVerMotion;
		Vector3 vHorMotion = thisTransform.right * fHorSpeed * Time.deltaTime * fHorMotion;
		Vector3 vGlobalMotion = vHorMotion + vVerMotion;
		
		// Se realiza la rotación relativo a las coordenadas del mundo global
		thisTransform.Rotate( vTurnMotion, Space.World);

		// Si realiza algún movimiento de traslación se aplica
		if ((fHorMotion != 0) || (fVerMotion != 0))  
		{
			// Se comprueba si el desplazamiento sobrepasa los límites globales impuestos. El objeto no se podrá
			// desplazar más allá de los límites.
			vGlobalMotion = new Vector3( Mathf.Clamp( vGlobalMotion.x, limits.x - thisTransform.position.x, 
				                                      limits.x + limits.width - thisTransform.position.x), 
				                         vGlobalMotion.y,
				                         Mathf.Clamp( vGlobalMotion.z, limits.y - limits.height - thisTransform.position.z, 
				                                      limits.y - thisTransform.position.z));
			
			// Se realiza el desplazamiento relativo a las coordenadas del mundo global
			thisTransform.Translate( vGlobalMotion, Space.World);
		}
	}
}

