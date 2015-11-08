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
	public int iMaxEnergy = 100;  // Energía máxima posible del objeto
	public int iSnapEnergy = 5;   // Salto de energía que se resta al recibir un impacto.

	int iEnergy;                  // Energía actual del objeto
	
	Animation thisAnimation;
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
	 * FUNCION: public void SetEnergy( int iNewEnergy)                       *
	 * DESCRIPCION: Fija el valor de la energía actual del objeto.           *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: int iNewEnergy: Valor de la nueva energía del objeto.      *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void SetEnergy( int iNewEnergy)
	{
		if (iNewEnergy > 0)
			iEnergy = iNewEnergy;
	}

	
	
	// Use this for initialization
	void Awake () 
	{
		iEnergy = iMaxEnergy;   // El objeto empieza con energía máxima
		
		thisAnimation = animation;	
		thisAudioSource = audio;
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
	void OnTriggerEnter( Collider obj)
	{
		// Si contacta una bala se le resta la energía
		if (obj.CompareTag( "Bullet"))
		{
			iEnergy = (iEnergy >= iSnapEnergy) ? (iEnergy - iSnapEnergy) : 0;
			thisAnimation.Play();
			thisAudioSource.Play();
			
			Destroy( obj);
		}
		
	}
}
