using UnityEngine;
using System.Collections;


/*************************************************************************
 * CLASE: public class Bomb : MonoBehaviour                              *
 * DESCRIPCION: Clase encargada de gestionar el estado de las bombas una *
 *   vez que son creadas y lanzadas.                                     *
 * ***********************************************************************/
public class Bomb : MonoBehaviour 
{
	enum StatusBomb {alive, explosion, death};  // Estados posibles de la bomba
	StatusBomb statusBomb;         // Estado actual de la bomba
	GameObject _objUser;           // Objeto usuario que ha lanzado esta bomba
	AttackUser attackUser;         // Componente Script Attack del objeto usuario

	public float fMaxTimeLive = 4.0f;       // Tiempo máximo de vida de la bomba en segundos
	float fTimeLive;                        // Tiempo transcurrido de vida de la bomba.
	public float fMaxTimeExplosion = 0.5f;  // Tiempo máximo de duración de la explosión
	float fTimeExplosion;                   // Tiempo transcurrido en la explosión
	public int iExplosionEnergy = 15;       // Energía a quitar al enemigo al explotar la bomba.
	public float fRateShockwave = 6.0f;     // Factor de la onda expansiva.
	
	SphereCollider thisSphereCollider;
	Rigidbody thisRigidbody;
	GameObject thisGameObject;
	Transform thisTransform;
	
	
	public GameObject objUser
	{
		set {_objUser = value;}
	}
	
	
	
	// Use this for initialization
	void Start () 
	{
		// Se inicializan las variables
		fTimeLive = 0.0f;
		fTimeExplosion = 0.0f;
		statusBomb = StatusBomb.alive;
		
		// Se obtienen el componente Attack del objeto usuario que lanzó esta bomba
		attackUser = _objUser.GetComponent<AttackUser>();
		
		thisSphereCollider = (SphereCollider)collider;
		thisRigidbody = rigidbody;
		thisGameObject = gameObject;
		thisTransform = transform;
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
				thisRigidbody.isKinematic = true;
				thisSphereCollider.isTrigger = true; 
				thisTransform.localScale *= fRateShockwave;
				
				// Se desactivan las físicas porque al activar el trigger, no funcionan las colisiones. 
				// De esta manera se evitan problemas al no estar afectado por las físicas.
			}
			break;
			
		case StatusBomb.explosion:
			// Pasado el tiempo máximo de explosión, se destruye el objeto bomba.		
			fTimeExplosion += Time.deltaTime;
			if (fTimeExplosion > fMaxTimeExplosion)
			{
				statusBomb = StatusBomb.death;
				attackUser.RemoveBomb( thisRigidbody);   // Se elimina la bomba de la lista
				Destroy( thisGameObject);            // Se destruye el objeto bomba
				
			}
			break;
			
		default:
			break;
		}
			
	
	}
	
	
	
	void OnTriggerEnter( Collider obj)
	{
		// Si la bomba alcanza un objeto de tipo target
		if (obj.CompareTag( "Enemy"))
		{
			// La onda expansiva alcanza el objeto quitándole energía.
			StatusBot statusBot = obj.GetComponent<StatusBot>();
			statusBot.HitObject( _objUser, iExplosionEnergy);
		}
	}
}