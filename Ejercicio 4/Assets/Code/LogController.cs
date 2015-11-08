using UnityEngine;
using System.Collections;

/*************************************************************************
 * CLASE: public class LogController : MonoBehaviour                     *
 * DESCRIPCION: Controlador de de realizar el seguimiento del movimiento *
 *   de un objeto controlado por el script MotionController.             *
 * ***********************************************************************/
public class LogController : MonoBehaviour
{
	public MotionController motionController;   // Referencia al script controlador del movimiento
	
	
	// Use this for initialization
	void Start ()
	{
		// Si no se ha asignado ningún script que seguir, se asigna el script del objeto actual.
		if (motionController == null)
			motionController = gameObject.GetComponent<MotionController>();
	
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{		
		MotionController.HorMotion horMotion;
		MotionController.VerMotion verMotion;
		MotionController.TurnMotion turnMotion;
		float fHorMotion, fVerMotion, fTurnMotion;

		motionController.GetDigitalMotion( out horMotion, out verMotion, out turnMotion);
		motionController.GetAnalogMotion( out fHorMotion, out fVerMotion, out fTurnMotion);

		switch (verMotion)
		{
			case MotionController.VerMotion.back:
				Debug.Log( "VERTICAL-(Back) ");
				break;
			case MotionController.VerMotion.forward:
				Debug.Log( "VERTICAL-(Forward) ");
				break;
			case MotionController.VerMotion.idle:
				Debug.Log( "VERTICAL-(Idle) ");
				break;
			default:
				break;
		}
	
		switch (horMotion)
		{
			case MotionController.HorMotion.right:
				Debug.Log( "HORIZONTAL-(Right) ");
				break;
			case MotionController.HorMotion.left:
				Debug.Log( "HORIZONTAL-(Left) ");
				break;
			case MotionController.HorMotion.idle:
				Debug.Log( "HORIZONTAL-(Idle) ");
				break;
			default:
				break;
		}

		switch (turnMotion)
		{
			case MotionController.TurnMotion.right:
				Debug.Log( "TURN-(Right) ");
				break;
			case MotionController.TurnMotion.left:
				Debug.Log( "TURN-(Left) ");
				break;
			case MotionController.TurnMotion.idle:
				Debug.Log( "TURN-(Idle) ");
				break;
			default:
				break;
		}

		// Se obtiene el tiempo en segundos que ha transcurrido en la acción actual.
		if (motionController.modeMotion == MotionController.ModeMotion.auto)
		{
			Debug.Log( "Time: " + motionController.SpentMotionTime());
		}
	}
}

