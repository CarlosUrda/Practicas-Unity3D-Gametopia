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
		// Se obtiene el tiempo en segundos que ha transcurrido en la acción actual.
		float fSpentMotionTime = motionController.SpentMotionTime();
		
		switch (motionController.GetCurrentMotion())
		{
		case MotionController.Motion.back:
			Debug.Log( "Back - Time: " + fSpentMotionTime);
			break;
		case MotionController.Motion.forward:
			Debug.Log( "Forward - Time: " + fSpentMotionTime);
			break;
		case MotionController.Motion.idle:
			Debug.Log( "Idle");
			break;
		default:
			break;
		}
	
	}
}

