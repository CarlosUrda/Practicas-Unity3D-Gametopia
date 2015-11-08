using UnityEngine;
using System.Collections;

public class MotionUser : MonoBehaviour 
{
	public float fHorSpeed;             // Velocidad del movimiento	de traslación horizontal (lateral)
	public float fVerSpeed;             // Velocidad del movimiento	de traslación vertical (adelante y atrás)
	public float fMouseSensX;           // Sensiblidad al mover el ratón en el eje X (para vista en 1 persona)

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

	Vector3 vVerMotion;                 // Vectores de desplazamiento que contendrán de manera
	Vector3 vHorMotion;                 //  temporal los vectores de desplazamiento y rotación
	Vector3 vGlobalMotion;              //  a realizar en cada frame.
	Vector3 vTurnMotion;

	Transform thisTransform;            // Referencia al componente Transform del objeto
	public MotionCamera motionCamera;   // Referencia al componente MotionCamera de la cámara principal.
	
	
	
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
	 * FUNCION: void ConfigManualDigitalMotion()                             *
	 * DESCRIPCION: Función encargada de obtener el nuevo tipo de movimiento *
	 *   digital, introducido por el usuario por teclado, que va a realizar  *
	 *   el objeto en la siguiente acción. Es decir, se encarga de obtener   *
	 *   que teclas se han pulsado en cada acción                            *
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
	}



	// Use this for initialization
	void Start ()
	{					
		thisTransform = transform;   // Se obtiene la referencia al componente Transform
		
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
		// Se obtiene la entrada digital del usuario para realizar el movimiento
		ConfigManualDigitalMotion();
		
		// Se obtienen los valores actuales del movimiento analógico real a partir de
		// los ejes proporcionados por Unity
		fHorMotion = Input.GetAxis( "Horizontal");
		fVerMotion = Input.GetAxis( "Vertical");

		// Si la cámara es en primera persona
		if (motionCamera.cameraPersp == MotionCamera.CameraPersp.first)
		{
			fTurnMotion = Input.GetAxis( "Mouse X") * fMouseSensX;			
			
			// Se obtienen los vectores de desplazamiento a partir del movimiento analógico real actual
			vVerMotion = thisTransform.forward * fVerSpeed * Time.deltaTime * fVerMotion;
			vHorMotion = thisTransform.right * fHorSpeed * Time.deltaTime * fHorMotion;
			vGlobalMotion = vHorMotion + vVerMotion;
			vTurnMotion = thisTransform.up * Time.deltaTime * fTurnMotion * fMouseSensX;
		
			// Se realiza la rotación relativo a las coordenadas del mundo global
			thisTransform.Rotate( vTurnMotion, Space.World);
		}
		// Si la cámara es isomética o cenital
		else
		{
			// Se obtienen los vectores de desplazamiento a partir del movimiento analógico real actual
			vVerMotion = Vector3.forward * fVerSpeed * Time.deltaTime * fVerMotion;
			vHorMotion = Vector3.right * fHorSpeed * Time.deltaTime * fHorMotion;
			vGlobalMotion = vHorMotion + vVerMotion;
						
			RaycastHit info;		
			Ray cameraRay = Camera.main.ScreenPointToRay( Input.mousePosition);
			if (Physics.Raycast( cameraRay, out info, 1000))
			{
				thisTransform.LookAt( new Vector3( info.point.x, thisTransform.position.y, info.point.z));
			}
			
		}
			
		// Se realiza el desplazamiento relativo a las coordenadas del mundo global
		thisTransform.Translate( vGlobalMotion, Space.World);
	}
}
