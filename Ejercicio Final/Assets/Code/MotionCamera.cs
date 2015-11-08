using UnityEngine;
using System.Collections;

public class MotionCamera : MonoBehaviour 
{
	public GameObject objUser;          // Objeto Usuario
	public GameObject objFloor;         // Escenario donde se desarrolla el juego

	// Tipos de posibles posiciones en las que pueda estar la cámara
	public enum CameraPersp {none = -1, first, cenital, iso};  // Primera persona; Cenital
	CameraPersp _cameraPersp;           // Perspectiva actual de la cámara.
	string _strCameraPersp;             // Perspectiva actual de la cámara en tipo string
	CameraPersp oldCameraPersp;         // Perspectiva de la cámara en el anterior frame.
	
	Transform thisTransform;            // Componente Transform de la cámara
	Transform floorTransform;           // Componente Transform del suelo
	Transform userTransform;            // Componente Transform del jugador
	Camera thisCamera;
	
	const float fHeight = 30.0f;        // Altura a la que se posicionará la cámara
	Bounds boundsFloor;                 // Límites del suelo
	Bounds boundsUser;                  // Limites del jugador
		
	
	
	public CameraPersp cameraPersp
	{
		get {return _cameraPersp;}
		set { _cameraPersp = value;}
	}
	

	
	public string strCameraPersp
	{
		get 
		{
			switch (_cameraPersp)
			{
			case MotionCamera.CameraPersp.cenital:
				_strCameraPersp = "Cenital";
				break;
			case CameraPersp.first:
				_strCameraPersp = "First Person";
				break;
			case CameraPersp.iso:
				_strCameraPersp = "Isometrics";
				break;
			case CameraPersp.none:
				_strCameraPersp = "Ninguna";
				break;
			default:
				break;
			}
			
			return _strCameraPersp;
		}		
	}
	

		
	// Use this for initialization
	void Start () 
	{
		// Se obtienen las referencias a los componentes del objeto y la cámara.
		thisTransform = transform;	
		thisCamera = camera;
		userTransform = objUser.transform;
		floorTransform = objFloor.transform;
		
		// Se inicializa la perspectiva anterior de la cámara
		oldCameraPersp = CameraPersp.none;
		
		// Se obtienen los límites tanto del jugador como del suelo.
		boundsUser = userTransform.renderer.bounds;
		boundsFloor = floorTransform.renderer.bounds;
	}
		
	
	
	// Update is called once per frame
	void Update()
	{
		switch (_cameraPersp)
		{		
		case CameraPersp.cenital:
			if (oldCameraPersp != CameraPersp.cenital)  // Si el frame anterior la cámara era cenital
			{
				thisCamera.orthographic = true;
				thisTransform.position = floorTransform.position + floorTransform.up * fHeight;
				thisTransform.LookAt( floorTransform);
				oldCameraPersp = CameraPersp.cenital;
			}
			break;
			
		case CameraPersp.first:
			thisCamera.orthographic = false;
			// Se situa la cámara en primera persona. Se posiciona en el lugar del extremo de la pistola.
			thisTransform.position = userTransform.position + userTransform.forward * boundsUser.extents.z;
			thisTransform.rotation = userTransform.rotation;
			oldCameraPersp = CameraPersp.first;
			break;
						
		case CameraPersp.iso:
			if (oldCameraPersp != CameraPersp.iso)  // Si el frame anterior la cámara era isométrica
			{
				thisCamera.orthographic = true;
				thisTransform.position = floorTransform.position + floorTransform.forward * boundsFloor.extents.z + 
				                         floorTransform.right * boundsFloor.extents.x + floorTransform.up * fHeight;
				thisTransform.LookAt( floorTransform);
				oldCameraPersp = CameraPersp.iso;
			}
			break;
			
		default:
			break;
		}
	}
	
}
