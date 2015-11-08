using UnityEngine;
using System.Collections;

public class ScreenInterface : MonoBehaviour 
{		
	// Variables relacionadas con la interface
	const int _iWidthScreen = 1920;    // Resolución de pantalla de referencia para
	const int _iHeightScreen = 1080;   // las coordenadas de los elementos de la interfaz

	// Dimensiones de la energía del jugador
	const int iWidthUserEnergy = 400;
	const int iHeightUserEnergy = 50;
	const int iXUserEnergy = 30;
	const int iYUserEnergy = _iHeightScreen - iHeightUserEnergy - 10;
	
	// Dimensiones de la energía del cargador de la pistola
	const int iWidthGunEnergy = iWidthUserEnergy;
	const int iHeightGunEnergy = iHeightUserEnergy;
	const int iXGunEnergy = iXUserEnergy;
	const int iYGunEnergy = iYUserEnergy - iHeightGunEnergy - 10;

	// Dimensiones del texto Número de enemigos existentes
	const int iWidthTextEnemies = 400;
	const int iHeightTextEnemies = 50;
	const int iXTextEnemies = 10;
	const int iYTextEnemies = 10;

	// Dimensiones del texto Número de enemigos matados
	const int iWidthTextKilled = 400;
	const int iHeightTextKilled = 50;
	const int iXTextKilled = iXTextEnemies;
	const int iYTextKilled = iYTextEnemies + iHeightTextKilled;

	// Dimensiones del texto Record
	const int iWidthTextRecord = 400;
	const int iHeightTextRecord = 50;
	const int iXTextRecord = iXTextEnemies;
	const int iYTextRecord = iYTextKilled + iHeightTextRecord;

	// Dimensiones del botón para cambiar de perspectiva
	const int iWidthButtonPersp = 400;
	const int iHeightButtonPersp = 50;
	const int iXButtonPersp = _iWidthScreen - 300;
	const int iYButtonPersp = 30;

	// Dimensiones del texto tipo de perspectiva
	const int iWidthTextPersp = 400;
	const int iHeightTextPersp = 50;
	const int iXTextPersp = iXButtonPersp + 10;
	const int iYTextPersp = iYButtonPersp + iHeightButtonPersp + 20;

	int iTempWidth;         // Variable Ancho temporal
	int iTempHeight;        // Variable Alto temporal
	float fRatioX;          // Relación entre la resolución virtual de referencia 
	float fRatioY;          // y la resolución actual de pantalla.
		
	// Elementos de UIToolKit
	UIStateSprite barUserEnergy;   // Barra de energía del usuario
	UIStateSprite barGunEnergy;    // Barra de energía del cargador de la pistola
	UIText textEnemies;            // Número de enemigos en pantalla
	UITextInstance textEnemiesInstance;
	UIText textKilled;             // Número de enemigos matados
	UITextInstance textKilledInstance;
	UIText textRecord;             // Máximo número de enemigos en anteriores partidas
	UITextInstance textRecordInstance; 
	UIButton buttonPersp;
	UIText textPersp;
	UITextInstance textPerspInstance; 
	
	Stage stage;                // Referencia al componente Stage.
	StatusUser statusUser;      // Referencia al componente StatusUser para información del estado del jugador
	MotionUser motionUser;      // Referencia al componente MotionUser para información de los movimientos del jugador.
	AttackUser attackUser;      // Referencia al componente AttackUser para información de los datos de ataque.
	MotionCamera motionCamera;  // Referencia al complemento MotionCamera del objeto Cámara principal

	
	

	public int iHeightScreen
	{
		get {return _iHeightScreen;}
	}
	
	
	
	public int iWidthScreen
	{
		get	{return _iWidthScreen;}
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		stage = GameObject.Find( "Floor").GetComponent<Stage>();
		GameObject objUser = GameObject.Find( "User");
		statusUser = objUser.GetComponent<StatusUser>();
		motionUser = objUser.GetComponent<MotionUser>();
		attackUser = objUser.GetComponent<AttackUser>();
		motionCamera = Camera.mainCamera.GetComponent<MotionCamera>();

		// Se calcula la relación entre la resolución virtual de referencia y la resolución actual de pantalla
		fRatioX = Screen.width * 1.0f / iWidthScreen;
		fRatioY = Screen.height * 1.0f / iHeightScreen;

		// Se crea la barra de energía y se inicializan los parámetros relacionados.		
		barUserEnergy = UIStateSprite.create( "Energy.png", 0, 0);
		barUserEnergy.hidden = false;
		barGunEnergy = UIStateSprite.create( "Level.png", 0, 0);
		barGunEnergy.hidden = false;
		textKilled = new UIText( "prototype", "prototype.png");
		textEnemies = new UIText( "prototype", "prototype.png");
		textRecord = new UIText( "prototype", "prototype.png");
		textEnemiesInstance = textEnemies.addTextInstance( "Enemigos: 0", iXTextEnemies * fRatioX, iYTextEnemies * fRatioY, 0.8f, 1, Color.green);
		textKilledInstance = textKilled.addTextInstance( "Asesinatos: 0", iXTextKilled * fRatioX, iYTextKilled * fRatioY, 0.8f, 1, Color.red);
		textRecordInstance = textRecord.addTextInstance( "Record: " + PlayerPrefs.GetInt( "Record", 0),
		                                                 iXTextRecord * fRatioX, iYTextRecord * fRatioY, 0.8f, 1, Color.black);
		buttonPersp = UIButton.create( "emptyUp.png", "emptyDown.png", 0, 0);
		buttonPersp.hidden = false;
		textPersp = new UIText( "prototype", "prototype.png");
		textPerspInstance = textPersp.addTextInstance( motionCamera.strCameraPersp, iXTextPersp * fRatioX, iYTextPersp * fRatioY, 0.9f, 1, Color.blue);
			
	}
	
	
	
	void OnButtonPerspClicked( UIButton uiButton)
	{
		motionCamera.cameraPersp = (MotionCamera.CameraPersp)(((int)motionCamera.cameraPersp + 1) % 3);
	}

	
	
	// Update is called once per frame
	void Update () 
	{
		// Se calcula la relación entre la resolución virtual de referencia y la resolución actual de pantalla
		fRatioX = Screen.width * 1.0f / _iWidthScreen;
		fRatioY = Screen.height * 1.0f / _iHeightScreen;

		// Ancho y alto de la barra de energía del usario en función de la resolución de pantalla.
		iTempWidth = (int)(iWidthUserEnergy * fRatioX);
		iTempHeight = (int)(iHeightUserEnergy * fRatioY);

		// Se muestra la energía del usuario
		barUserEnergy.setSize( (int)(iTempWidth * (statusUser.iEnergy * 1.0f / statusUser.iMaxEnergy)), iTempHeight);
		barUserEnergy.pixelsFromTopLeft( (int)(iYUserEnergy * fRatioY), (int)(iXUserEnergy * fRatioX));
		
		// Ancho y alto de la barra de energía del disparo en función de la resolución de pantalla.
		iTempWidth = (int)(iWidthGunEnergy * fRatioX);
		iTempHeight = (int)(iHeightGunEnergy * fRatioY);

		// Se muestra la energía del cargador de la pistola
		barGunEnergy.setSize( (int)(iTempWidth * (attackUser.iLevelShot * 1.0f / attackUser.iMaxLevelShot)), iTempHeight);
		barGunEnergy.pixelsFromTopLeft( (int)(iYGunEnergy * fRatioY), (int)(iXGunEnergy * fRatioX));
		
		textEnemiesInstance.text = "Enemigos: " + stage.listEnemies.Count;	
		textEnemiesInstance.pixelsFromTopLeft( (int)(iYTextEnemies * fRatioY), (int)(iXTextEnemies * fRatioX));
		textKilledInstance.text = "Asesinatos: " + statusUser.iEnemiesKilled;
		textKilledInstance.pixelsFromTopLeft( (int)(iYTextKilled * fRatioY), (int)(iXTextKilled * fRatioX));
		textRecordInstance.pixelsFromTopLeft( (int)(iYTextRecord * fRatioY), (int)(iXTextRecord * fRatioX));

		buttonPersp.pixelsFromTopLeft( (int)(iYButtonPersp * fRatioY), (int)(iXButtonPersp * fRatioX));
		textPerspInstance.text = motionCamera.strCameraPersp;
		textPerspInstance.pixelsFromTopLeft( (int)(iYTextPersp * fRatioY), (int)(iXTextPersp * fRatioX));

		buttonPersp.onTouchDown += OnButtonPerspClicked;
	}
}
