using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	GameObject objUser;       // Objeto usuario que ha disparado esta bala
	Attack attack;                   // Componente Script Attack del objeto usuario

	const float fMaxTime = 5.0f;   // Tiempo máximo de vida de la bala en segundos
	const int iImpactEnergy = 10;  // Energía a quitar el en impacto de la bala.
	float fTime = 0.0f;            // Tiempo de vida de la bala.
	
	
	
	// Use this for initialization
	void Start () 
	{
		fTime = 0.0f;
		attack = objUser.GetComponent<Attack>();
	}
	
	
	
	/*************************************************************************
	 * FUNCION: void SetObjUser( GameObject obj)                             *
	 * DESCRIPCION: Función encargada de asignar el objeto manejado por el   *
	 *   usuario que ha disparado esta bala.                                 *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: obj: Objeto del usuario que disparó esta bala              *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno.                                                     *
	 *************************************************************************/	
	void SetObjUser( GameObject obj)
	{
		objUser = obj;
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		// Pasado el tiempo máximo de vida se destruye la bala.
		fTime += Time.deltaTime;
		if (fTime > fMaxTime)
		{
			attack.listBullets.Remove( gameObject.rigidbody);
			Destroy( gameObject);
		}
	
	}
	
	
	
	void OnTriggerEnter( Collider obj)
	{
		// Si la bala contacta con un objeto de tipo target u objetivo
		if (obj.CompareTag( "Target"))
		{
			obj.SendMessage( "HitObject", iImpactEnergy);  // Se impacta al objeto para quitar 10 de energía
		}
		
		// Al haber contactado con cualquier objeto se elimina la bala de la lista de balas, y se destruye
		attack.listBullets.Remove( gameObject.rigidbody);
		Destroy( gameObject);
		
	}

}
