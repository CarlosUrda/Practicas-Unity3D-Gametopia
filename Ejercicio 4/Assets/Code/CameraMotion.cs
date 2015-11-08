using UnityEngine;
using System.Collections;

public class CameraMotion : MonoBehaviour 
{
	public GameObject objectGUI;          // Objeto a partir del cual se realiza la interface.
	Transform thisTransform;              // Componente Transform de la cámara
	Camera thisCamera;
	MotionController objMotionController;
		
	
	
	// Use this for initialization
	void Start () 
	{
		// Se obtienen las referencias a los componentes del objeto y la cámara.
		objMotionController = objectGUI.GetComponent<MotionController>();
		thisTransform = transform;	
		thisCamera = camera;

		// Situamos la cámara en función de la posición y dimensiones de los límites por donde
		// puede moverse el objeto, de manera que pueda enfocar todo el espacio abarcado por los límites
		// independientemente de las dimensiones de estos.
		thisTransform.position = new Vector3( objMotionController.limits.x + objMotionController.limits.width / 2.0f,
			                                  1.5f * Mathf.Max( objMotionController.limits.width, objMotionController.limits.height) / 
			                                  Mathf.Tan( thisCamera.fieldOfView * Mathf.Deg2Rad),
			                                  objMotionController.limits.y - objMotionController.limits.height / 2.0f);
	}
		
	
	
	// Update is called once per frame
	void Update()
	{
	}
		
}
