using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*************************************************************************
 * CLASE: public class Attack : MonoBehaviour                            *
 * DESCRIPCION: Clase encargada de manejar el disparo de balas desde el  *
 *   objeto que es manejado por el usuario.                              *
 * ***********************************************************************/
public class Attack : MonoBehaviour 
{
	public int iMaxShotLevel = 100;    // Máxima energía que puede alcanzar el disparo
	public Rigidbody prefabBullet;     // Prefab de la bala a partir de la cual se crearán 
	                                   // el resto de proyectiles a disparar.
	public int iMaxForce = 20000;      // Máxima fuerza con la que salen las balas disparadas.   
	public float fSlopeShot = 0.5f;    // Inclinación de la pistola para que el disparo no salga plano
	                                   // sino que salga haciendo una parábola. 
	
	int iSnapShotLevel = 5;            // Salto en el incremento de la energía de disparo.
	int iShotLevel;                    // Energía del disparo actual
	public List <Rigidbody> listBullets;      // Lista de balas existentes en la partida
	
	

	Transform thisTransform;           // Referencia al componente Transform del objeto

	
	
	/*************************************************************************
	 * FUNCION: public int GetShotLevel()                                    *
	 * DESCRIPCION: Obtener el nivel de energía del disparo actual.          *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: int: Nivel del disparo actual                                *
	 *************************************************************************/
	public int GetShotLevel()
	{
		return iShotLevel;
	}

	
	
	/*************************************************************************
	 * FUNCION: public void SetEnergy( int iNewEnergy)                       *
	 * DESCRIPCION: Fija el valor del nivel de energía del disparo actual    *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: int iNewShotLevel: Valor del nuevo nivel de disparo        *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void SetShotLevel( int iNewShotLevel)
	{
		if ((iNewShotLevel >= 0) && (iNewShotLevel <= iMaxShotLevel))
			iShotLevel = iNewShotLevel;
	}

	
			
	// Use this for initialization
	void Start () 
	{
		iShotLevel = 0;
		thisTransform = transform;
		listBullets = new List<Rigidbody>();  
	
	}

	
	
	// Update is called once per frame
	void Update () 
	{
		Rigidbody tmpBullet;
		
		// Si el botón de disparo se suelta, se procede a disparar
		if (Input.GetButtonUp( "Fire"))
		{
			tmpBullet = Instantiate( prefabBullet, thisTransform.position, thisTransform.rotation) as Rigidbody;
			tmpBullet.SendMessage( "SetObjUser", gameObject);  // Se fija el objeto que creo esta bala.
			listBullets.Add( tmpBullet);  // Se añade la bala a la lista de balas existentes.
			tmpBullet.AddForce( (thisTransform.forward + new Vector3( 0, fSlopeShot, 0)) * iMaxForce * (iShotLevel * 1.0f / iMaxShotLevel));
			iShotLevel = 0;
			
		}
		else
		{
			// Se actualiza el nivel de disparo dependiendo si se está pulsando el botón o no.
			int iTempSnap = Input.GetButton( "Fire") ?	iSnapShotLevel : -iSnapShotLevel;		
			iShotLevel = Mathf.Clamp( iShotLevel + iTempSnap, 0, iMaxShotLevel);
		}
			
	}
}
