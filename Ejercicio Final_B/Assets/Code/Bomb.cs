using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*************************************************************************
 * CLASE: public class Bomb : MonoBehaviour                              *
 * DESCRIPCION: Clase encargada de gestionar el estado de las bombas una *
 *   vez que son creadas y lanzadas.                                     *
 * ***********************************************************************/
public class Bomb : MonoBehaviour 
{
	enum StatusBomb {alive, explosion, death};  // Estados posibles de la bomba
	StatusBomb statusBomb;         // Estado actual de la bomba
	GameObject objUser;            // Objeto usuario que ha lanzado esta bomba
	Attack attack;                 // Componente Script Attack del objeto usuario

	const float fMaxTimeLive = 5.0f;       // Tiempo máximo de vida de la bomba en segundos
	float fTimeLive;                       // Tiempo transcurrido de vida de la bomba.
	const float fMaxTimeExplosion = 1.0f;  // Tiempo máximo de duración de la explosión
	float fTimeExplosion;                  // Tiempo transcurrido en la explosión
	const int iExplosionEnergy = 15;       // Energía a quitar al enemigo al explotar la bomba.
	const float fRateShockwave = 3.0f;     // Factor para calcular la onda expansiva a partir del radio
	
	List <Collider> listObjsShockwave;     // Lista de objetos que han sido alcanzados por la onda expansiva de la bomba
	
	SphereCollider thisSphereCollider;
	Rigidbody thisRigidbody;
	GameObject thisGameObject;
	
	
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
	
	
	
	// Use this for initialization
	void Start () 
	{
		// Se inicializan las variables
		fTimeLive = 0.0f;
		fTimeExplosion = 0.0f;
		statusBomb = StatusBomb.alive;
		
		// Se obtienen el componente Attack del objeto usuario que lanzó esta bomba
		attack = objUser.GetComponent<Attack>();
		
		listObjsShockwave = new List<Collider>();
		
		thisSphereCollider = (SphereCollider)collider;
		thisRigidbody = rigidbody;
		thisGameObject = gameObject;
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		switch (statusBomb)
		{			
		case StatusBomb.alive:			
			// Pasado el tiempo máximo de vida, explota la bomba.					
			fTimeLive += Time.deltaTime;
			if (fTimeLive > fMaxTimeLive)
			{
				statusBomb = StatusBomb.explosion;
				fTimeExplosion = 0.0f;

				// SE CREA LA ANIMACION DE LA EXPLOSION Y SE REPRODUCE EL SONIDO
				
				// Se activa el trigger y se desactiva el motor de físicas.
				thisSphereCollider.isTrigger = true; 
				thisSphereCollider.radius *= fRateShockwave;
				
				// Se desactivan las físicas porque al activar el trigger, no funcionan las colisiones. 
				// De esta manera se evitan problemas al no estar afectado por las físicas.
				thisRigidbody.isKinematic = true;				
			}
			break;
			
		case StatusBomb.explosion:
			// Pasado el tiempo máximo de explosión, se destruye el objeto bomba.		
			fTimeExplosion += Time.deltaTime;
			if (fTimeExplosion > fMaxTimeExplosion)
			{
				statusBomb = StatusBomb.death;
				attack.RemoveBomb( thisRigidbody);   // Se elimina la bomba de la lista
				Destroy( thisGameObject);            // Se destruye el objeto bomba
				
			}
			break;
			
		default:
			break;
		}
			
	
	}
	
	
	
	void OnTriggerEnter( Collider obj)
	{
		// Se comprueba si el objeto ya había sido alcanzado por la onda expansiva
		if (listObjsShockwave.IndexOf( obj) == -1)
		{
			// Si la bomba alcanza con un objeto de tipo target
			if (obj.CompareTag( "Enemy"))
			{
				obj.SendMessage( "HitObject", objUser, iExplosionEnergy);  // Se impacta al objeto para quitar 10 de energía
				listObjsShockwave.Add( obj);
			}
		}
	}
}