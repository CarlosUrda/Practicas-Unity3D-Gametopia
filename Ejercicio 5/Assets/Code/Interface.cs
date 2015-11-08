using UnityEngine;
using System.Collections;

public class Interface : MonoBehaviour 
{
	// Componentes scripts del objeto.
	public GUISkin guiSkin;
	public GameObject objUser;        // Objeto que maneja el usuario
	public GameObject objAI;          // Objeto que maneja la inteligencia artificial.

	// Referencias a los componentes
	Status objAIStatus;                          // Componente Estado del objeto AI
	Attack objUsrAttack;                         // Componente Attack del objeto usuario.
	MotionController objUserMotionController;    // Componente MotionController del objeto Usuario
	MotionController objAIMotionController;      // Componente MotionController del objeto AI
	CameraMotion cameraMotion;
	
	public Texture energy;               // Textura con la barra de energía
	public Texture level;                // Textura con la barra de nivel de disparo
	public Texture left, right, forward, back, idle;  // Texturas con las direcciones digitales de entrada.
	
	Vector3 vGUIScale;                // Vector para escalar la matriz de transformación del GUI
	
	const int iWidthScreen = 1920;    // Resolución de pantalla de referencia para
	const int iHeightScreen = 1080;   // las coordenadas de los elementos de la interfaz
	
	// Dimensiones de la caja exterior donde aparece la energía
	const int iWidthBoxEnergy = 400;
	const int iHeightBoxEnergy = 50;
	const int iXBoxEnergy = 30;
	const int iYBoxEnergy = 30;

	// Dimensiones de la caja interior donde aparece la energía
	const int iWidthEnergy = iWidthBoxEnergy - 20;
	const int iHeightEnergy = iHeightBoxEnergy - 10;
	const int iXEnergy = iXBoxEnergy + 10;
	const int iYEnergy = iYBoxEnergy + 5;

	// Dimensiones del texto indicando la energía
	const int iWidthLabelEnergy = iHeightBoxEnergy;
	const int iHeightLabelEnergy = iHeightBoxEnergy;
	const int iXLabelEnergy = iXBoxEnergy + iWidthBoxEnergy + 10;
	const int iYLabelEnergy = iYBoxEnergy;

	// Dimensiones de la caja exterior donde aparece el nivel de disparo
	const int iWidthBoxLevel = iWidthBoxEnergy;
	const int iHeightBoxLevel = iHeightBoxEnergy;
	const int iXBoxLevel = iXBoxEnergy;
	const int iYBoxLevel = iYBoxEnergy + iHeightBoxLevel + 10;

	// Dimensiones de la caja interior donde aparece el nivel de disparo
	const int iWidthLevel = iWidthBoxLevel - 20;
	const int iHeightLevel = iHeightBoxLevel - 10;
	const int iXLevel = iXBoxLevel + 10;
	const int iYLevel = iYBoxLevel + 5;

	// Dimensiones del texto indicando el nivel de disparo
	const int iWidthLabelLevel = iHeightBoxLevel;
	const int iHeightLabelLevel = iHeightBoxLevel;
	const int iXLabelLevel = iXBoxLevel + iWidthBoxLevel + 10;
	const int iYLabelLevel = iYBoxLevel;

	// Dimensiones del botón para cambiar el modo de perspectiva de la cámara
	const int iWidthButtonPersp = 100;
	const int iHeightButtonPersp = 50;
	const int iXButtonPersp = 1800;
	const int iYButtonPersp = 30;
	
	// Dimensiones del botón para cambiar la aleatoriedad del tiempo del movimiento
	const int iWidthButtonAleat = 100;
	const int iHeightButtonAleat = 50;
	const int iXButtonAleat = iXButtonPersp - iWidthButtonPersp - 5;
	const int iYButtonAleat = iYButtonPersp;

	// Dimensiones de la caja dirección horizontal
	const int iWidthBoxHorInput = 150;
	const int iHeightBoxHorInput = 30;
	const int iXBoxHorInput = 1700;
	const int iYBoxHorInput = 950;

	// Dimensiones de la caja dirección vertical
	const int iWidthBoxVerInput = iHeightBoxHorInput;
	const int iHeightBoxVerInput = iWidthBoxHorInput;
	const int iXBoxVerInput = iXBoxHorInput + iWidthBoxHorInput/2 - iWidthBoxVerInput/2;
	const int iYBoxVerInput = iYBoxHorInput - iWidthBoxHorInput/2 + iWidthBoxVerInput/2;

	// Dimensiones de la caja giro
	const int iWidthBoxTurnInput = iWidthBoxHorInput;
	const int iHeightBoxTurnInput = iHeightBoxHorInput;
	const int iXBoxTurnInput = 1500;
	const int iYBoxTurnInput = iYBoxHorInput;

	// Dimensiones de la caja donde aparece el movimiento analógico real
	const int iWidthBoxAnalog = 150;
	const int iHeightBoxAnalog = 150;
	const int iXBoxAnalog = 30;
	const int iYBoxAnalog = 890;

	// Dimensiones del elemento indicandor de la posición en la caja del movimiento analógico real.
	const int iWidthAnalog = iWidthBoxAnalog / 10;
	const int iHeightAnalog = iHeightBoxAnalog / 10;
	const int iXAnalog = iXBoxAnalog + iWidthBoxAnalog/2 - iWidthAnalog/2;
	const int iYAnalog = iYBoxAnalog + iHeightBoxAnalog/2 - iHeightAnalog/2;

	
	
	void Start()
	{
		// Se obtienen las referencias a los componentes del objeto.
		objUserMotionController = objUser.GetComponent<MotionController>();
		objAIMotionController = objAI.GetComponent<MotionController>();
		objAIStatus = objAI.GetComponent<Status>();
		objUsrAttack = objUser.GetComponent<Attack>();

		// Referencias a los componentes de la cámara
		cameraMotion = GetComponent<CameraMotion>();
		
		// Relación de aspecto entre la resolución de pantalla real y las resolución de referencia.
		vGUIScale = new Vector3( Screen.width / (iWidthScreen * 1.0f), Screen.height / (iHeightScreen * 1.0f), 1);
	}

	
	
	void OnGUI()
	{
		GUI.skin = guiSkin;
		MotionController.HorMotion horMotion;
		MotionController.VerMotion verMotion;
		MotionController.TurnMotion turnMotion;
		float fHorMotion, fVerMotion, fTurnMotion;

		// Se obtienen los datos del movimiento digital y analógico
		objUserMotionController.GetDigitalMotion( out horMotion, out verMotion, out turnMotion);
		objUserMotionController.GetAnalogMotion( out fHorMotion, out fVerMotion, out fTurnMotion);
				
		// Matriz de transformación para adaptar los elementos a la resolución real de pantalla.
		GUI.matrix = Matrix4x4.TRS( new Vector3( 0, 0, 1), Quaternion.identity, vGUIScale);

		// Se muestra la energía
		GUI.Box( new Rect( iXBoxEnergy, iYBoxEnergy, iWidthBoxEnergy, iHeightBoxEnergy), "");
		GUI.DrawTexture( new Rect( iXEnergy, iYEnergy, (iWidthEnergy * objAIStatus.GetEnergy()) * 1.0f / objAIStatus.iMaxEnergy, 
					               iHeightEnergy), energy);
		GUI.color = Color.white;
		GUI.Label( new Rect( iXLabelEnergy, iYLabelEnergy, iWidthLabelEnergy, iHeightLabelEnergy), 
			       "" + objAIStatus.GetEnergy());
		
		// Se muestra el nivel de energía del disparo
		GUI.Box( new Rect( iXBoxLevel, iYBoxLevel, iWidthBoxLevel, iHeightBoxLevel), "");
		GUI.DrawTexture( new Rect( iXLevel, iYLevel, (iWidthLevel * objUsrAttack.GetShotLevel()) * 1.0f / objUsrAttack.iMaxShotLevel, 
					               iHeightLevel), level);
		GUI.color = Color.white;
		GUI.Label( new Rect( iXLabelLevel, iYLabelLevel, iWidthLabelLevel, iHeightLabelLevel), 
			       "" + objUsrAttack.GetShotLevel());

		// Se muestra el Botón que modifica la perspectiva de la cámara
		GUI.color = Color.yellow;
		switch (cameraMotion.cameraPersp)
		{
			case CameraMotion.CameraPersp.first:
				if (GUI.Button( new Rect( iXButtonPersp, iYButtonPersp, iWidthButtonPersp, iHeightButtonPersp), "FIRST"))
					cameraMotion.cameraPersp = CameraMotion.CameraPersp.cenital;
				break;

			case CameraMotion.CameraPersp.cenital:
				if (GUI.Button( new Rect( iXButtonPersp, iYButtonPersp, iWidthButtonPersp, iHeightButtonPersp), "CENITAL"))
					cameraMotion.cameraPersp = CameraMotion.CameraPersp.first;
				break;

			default:
				break;		
		}
		
		// Se muestra el botón que indica si el tiempo del movimiento de la AI es aleatorio o fijo.
		if (objAIMotionController.bRandomTime)
		{
			if (GUI.Button( new Rect( iXButtonAleat, iYButtonAleat, iWidthButtonAleat, iHeightButtonAleat), 
				            "Random Time"))
				objAIMotionController.bRandomTime = false;	
		}
		else
		{
			if (GUI.Button( new Rect( iXButtonAleat, iYButtonAleat, iWidthButtonAleat, iHeightButtonAleat), "Fixed Time"))
				objAIMotionController.bRandomTime = true;	
		}

		
		// Se muestra los controles de entrada digitales.
		GUI.color = Color.white;
		Texture textureMotion = null;    // Variable que contendrá la textura correspondiente a la entrada digital.
		switch (horMotion)
		{
			case MotionController.HorMotion.left:
				textureMotion = left;
				break;
			case MotionController.HorMotion.right:
				textureMotion = right;
				break;
			// Se deja esta opción por si más adelante se quiere cambiar la textura en idle.        
			case MotionController.HorMotion.idle:
				textureMotion = idle;
				break;
			default:
				break;
		}
		GUI.DrawTexture( new Rect( iXBoxHorInput, iYBoxHorInput, iWidthBoxHorInput, iHeightBoxHorInput), textureMotion);
		
		switch (verMotion)
		{
			case MotionController.VerMotion.forward:
				textureMotion = forward;
				break;
			case MotionController.VerMotion.back:
				textureMotion = back;
				break;
			case MotionController.VerMotion.idle:
				textureMotion = idle;
				break;
			default:
				break;
		}
		GUI.DrawTexture( new Rect( iXBoxVerInput, iYBoxVerInput, iWidthBoxVerInput, iHeightBoxVerInput), textureMotion);

		switch (turnMotion)
		{
			case MotionController.TurnMotion.left:
				textureMotion = left;
				break;
			case MotionController.TurnMotion.right:
				textureMotion = right;
				break;
			case MotionController.TurnMotion.idle:
				textureMotion = idle;
				break;
			default:
				break;
		}		
		GUI.DrawTexture( new Rect( iXBoxTurnInput, iYBoxTurnInput, iWidthBoxTurnInput, iHeightBoxTurnInput), textureMotion);
					
		// Se muestra la caja con el control analógico real
		GUI.color = Color.blue;
		GUI.Box( new Rect( iXBoxAnalog, iYBoxAnalog, iWidthBoxAnalog, iHeightBoxAnalog), "");
		GUI.Box( new Rect( (int)(iXAnalog + fHorMotion * (iWidthBoxAnalog/2 - iWidthAnalog/2)), 
			               (int)(iYAnalog + -fVerMotion * (iHeightBoxAnalog/2 - iHeightAnalog/2)), 
			               iWidthAnalog, iHeightAnalog), "T");
		GUI.Box( new Rect( (int)(iXAnalog + fTurnMotion * iWidthBoxAnalog/2), (int)(iYAnalog + iHeightBoxAnalog/2 + iHeightAnalog), 
			               iWidthAnalog, iHeightAnalog), "R");

		
	}
}
