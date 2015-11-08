using UnityEngine;
using System.Collections;

public class CameraMotion : MonoBehaviour 
{
	public GameObject objAI;            // Objeto IA
	public GameObject objUser;          // Objeto Usuario
	
	// Tipos de posibles posiciones en las que pueda estar la cámara
	public enum CameraPersp {first, cenital};  // Primera persona; Cenital
	public CameraPersp cameraPersp;     // Perspectiva actual de la cámara.
	
	Transform thisTransform;            // Componente Transform de la cámara
	Camera thisCamera;
	MotionController objUserMotionController;
	Transform objUserTransform;
		
	
	
	// Use this for initialization
	void Start () 
	{
		// Se obtienen las referencias a los componentes del objeto y la cámara.
		objUserMotionController = objUser.GetComponent<MotionController>();
		objUserTransform = objUser.GetComponent<Transform>();
		thisTransform = transform;	
		thisCamera = camera;
		
		// Se inicializa la perspectiva de la cámara
		cameraPersp = CameraPersp.cenital;
	}
		
	
	
	// Update is called once per frame
	void Update()
	{
		switch (cameraPersp)
		{
			case CameraPersp.cenital:
				// Situamos la cámara en función de la posición y dimensiones de los límites por donde
				// puede moverse el objeto, de manera que pueda enfocar todo el espacio abarcado por los límites
				// independientemente de las dimensiones de estos.
				thisTransform.position = new Vector3( objUserMotionController.limits.x + objUserMotionController.limits.width / 2.0f,
					                                  1.5f * Mathf.Max( objUserMotionController.limits.width, objUserMotionController.limits.height) / 
					                                  Mathf.Tan( thisCamera.fieldOfView * Mathf.Deg2Rad),
					                                  objUserMotionController.limits.y - objUserMotionController.limits.height / 2.0f);
				thisTransform.eulerAngles = new Vector3( 90, 0, 0);
				break;
			
			case CameraPersp.first:
				// Se situa la cámara en primera persona.
				thisTransform.position = objUserTransform.position;
				thisTransform.rotation = objUserTransform.rotation;
				break;
			
			
			default:
				break;
		}
	}
		
}
