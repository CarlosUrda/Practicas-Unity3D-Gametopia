using UnityEngine;
using System.Collections;

public class MotionBot : MonoBehaviour 
{
	public float fMaxMotionTime = 4.0f; // Tiempo máximo que va a durar cada movimiento
	float fCurrentMotionTime;           // Tiempo que va a durar el movimiento actual de la máquina
	float fStartTime;                   // Momento de tiempo en que se inició el último movimiento de la máquina
	public bool bRandomTime = false;    // Flag para activar tiempo de duración aleatorio en cada movimiento
	public float fHorSpeed;             // Velocidad del movimiento	de traslación horizontal (lateral)
	public float fVerSpeed;             // Velocidad del movimiento	de traslación vertical (adelante y atrás)
	public float fTurnSpeed;            // Velocidad del movimiento	de rotación

	// Tipos posibles de movimientos digitales del teclado simulados por IA del objeto
	public enum HorMotion {left = -1, idle, right};    // Movimientos Horizontales
	public enum VerMotion {back = -1, idle, forward};  // Movimientos Verticales
	public enum TurnMotion {left = -1, idle, right};   // Tipos posibles de giros
	HorMotion horMotion;                // Entrada digital horizontal del movimiento actual
	VerMotion verMotion;                // Entrada digital vertical del movimiento actual
	TurnMotion turnMotion;              // Entrada de giro digital ejecutándose actualmente
	
	public bool bNewMotion;             // Flag para forzar un nuevo movimiento sin esperar el tiempo debido.
		
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

	Transform thisTransform;          // Referencia al componente Transform del objeto
	StatusBot statusBot;              // Referencia al componente StatusBot del objeto
	
	
	
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
	 * FUNCION: void ConfigAutoDigitalMotion()                               *
	 * DESCRIPCION: Función encargada de generar el nuevo tipo de movimiento *
	 *   digital, calculado por la máquina, que va a realizar el objeto en   *
	 *   la siguiente acción. El tipo de movimiento depende del estado de la *
	 *   acción actual del bot: normal, kamikaze o death.                    *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno (void)                                               *
	 *************************************************************************/
	void ConfigAutoDigitalMotion()
	{
		switch (statusBot.action)
		{
		case StatusBot.Action.normal:

			// Si el movimiento ha superado el tiempo de duración o se le fuerza a cambiar de movimiento
			// se configura un nuevo movimiento digital automático
			if (((Time.timeSinceLevelLoad - fStartTime) > fCurrentMotionTime) || bNewMotion)
			{
				bNewMotion = false;
				// Se obtiene el tiempo en segundos que va a durar el nuevo movimiento a realizar
				fCurrentMotionTime = (bRandomTime) ? Random.Range( 1, fMaxMotionTime + 1) : fMaxMotionTime;
				fStartTime = Time.timeSinceLevelLoad;  // Momento del tiempo en que se inicia el nuevo movimiento.
				
				// Se obtiene la nueva entrada de teclado digital como si la máquina pulsase las teclas de movimiento
				horMotion = (HorMotion) Random.Range( (int)HorMotion.left, (int)HorMotion.right + 1);
				verMotion = (VerMotion) Random.Range( (int)VerMotion.back, (int)VerMotion.forward + 1);
				turnMotion = (TurnMotion) Random.Range( (int)TurnMotion.left, (int)TurnMotion.right + 1);
			}

			break;
		
		case StatusBot.Action.kamikaze:
			// Vector dirección desde la posicion del bot a la posición del objeto usuario que se quiere kamikazear
			Vector3 vDirKamikaze = statusBot.objKamikaze.transform.position - thisTransform.position;

			// Se obtiene el movimiento vertical digital
			verMotion = (Vector3.Dot( thisTransform.forward, vDirKamikaze) >= 0) ? VerMotion.forward : VerMotion.back;
			
			// Movimiento digital horizontal y digital de giro
			float fDotHor;
			if ((fDotHor = Vector3.Dot( thisTransform.right, vDirKamikaze)) > 0)
			{
				turnMotion = (verMotion == VerMotion.forward) ? TurnMotion.right : TurnMotion.left;
				horMotion = HorMotion.right;
			}
			else if (fDotHor < 0)
			{
				turnMotion = (verMotion == VerMotion.forward) ? TurnMotion.left : TurnMotion.right;
				horMotion = HorMotion.left;
			}
			else
			{
				horMotion = HorMotion.idle;
				turnMotion = TurnMotion.idle;
			}
				
			break;
		
		case StatusBot.Action.death:
			horMotion = HorMotion.idle;
			verMotion = VerMotion.idle;
			turnMotion = TurnMotion.idle;
			break;
		default:
			break;			
		}
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
	
	
	
	// Use this for initialization
	void Start ()
	{					
		thisTransform = transform;               // Se obtiene la referencia al componente Transform
		statusBot = GetComponent<StatusBot>();   // Se obtiene la referencia al componente StatusBot
		
		// Se inicializan las variables
		fCurrentMotionTime = fStartTime = 0;
		bNewMotion = false;
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
		// Se configura el siguiente movimiento
		ConfigAutoDigitalMotion();

		// Se convierte el movimiento digital actual a movimiento real analógico.
		InputDigitalToRealAnalog();			

		// Se obtienen los vectores de desplazamiento a partir del movimiento analógico real actual
		Vector3 vTurnMotion = thisTransform.up * fTurnSpeed * Time.deltaTime * fTurnMotion;
		Vector3 vVerMotion = thisTransform.forward * fVerSpeed * Time.deltaTime * fVerMotion;
		Vector3 vHorMotion = thisTransform.right * fHorSpeed * Time.deltaTime * fHorMotion;
		Vector3 vGlobalMotion = vHorMotion + vVerMotion;
		
		// Se realiza la rotación relativo a las coordenadas del mundo global
		thisTransform.Rotate( vTurnMotion, Space.World);

		// Se realiza el desplazamiento relativo a las coordenadas del mundo global
		thisTransform.Translate( vGlobalMotion, Space.World);
	}
}
