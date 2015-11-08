using UnityEngine;
using System.Collections;

/*************************************************************************
 * CLASE: public class Status : MonoBehaviour                            *
 * DESCRIPCION: Clase encargada de gestionar el estado de energía del    *
 *   objeto manejado por la máquina.                                     *
 * ***********************************************************************/

[RequireComponent(typeof(AudioSource))]
public class Status : MonoBehaviour 
{
	public int iMaxEnergy = 100;        // Energía máxima posible del objeto
	public const int iSnapEnergy = 5;   // Salto de energía que se resta al recibir un impacto.
	public GameObject objCam;           // Objeto cámara del juego.
	Interface intfce;                   // Componente script Interface de la cámara del juego

	int iEnergy;                        // Energía actual del objeto
	
//	Animation thisAnimation;
	AudioSource thisAudioSource;
	
	
	
	/*************************************************************************
	 * FUNCION: public int GetEnergy()                                       *
	 * DESCRIPCION: Obtener la energía actual del objeto.                    *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: int: Energía del objeto                                      *
	 *************************************************************************/
	public int GetEnergy()
	{
		return iEnergy;
	}

	
	
	/*************************************************************************
	 * FUNCION: public void HitObject()                                      *
	 * DESCRIPCION: Función encargada de modificar el objeto en caso de      *
	 *   recibir un impacto.                                                 *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: int iSnap: Cantidad de energía a quitar del objeto.        *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void HitObject( int iSnap = iSnapEnergy)
	{
		// Se disminuye la energía
		iEnergy = (iEnergy >= iSnap) ? (iEnergy - iSnap) : 0;
		thisAudioSource.Play();
		
		if (iEnergy == 0)
		{
			intfce.bEndGame = true;   // Activamos el flag de fin de partida
			Destroy( gameObject);
		}
	}

	
	
	// Use this for initialization
	void Awake () 
	{
		iEnergy = iMaxEnergy;   // El objeto empieza con energía máxima
		
//		thisAnimation = animation;	
		thisAudioSource = audio;
		
		// Se obtiene el componente script Interface de la cámara del juego.
		intfce = objCam.GetComponent<Interface>(); 
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
}
