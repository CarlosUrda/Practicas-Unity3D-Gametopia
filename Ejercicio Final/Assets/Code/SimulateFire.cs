using UnityEngine;
using System.Collections;

/*************************************************************************
 * CLASE: public class SimulateFire : MonoBehaviour                      *
 * DESCRIPCION: Clase encargada de gestionar la simulación de los        *
 *   disparos: parábola del lanzamiento de bombas y simulación de las    *
 *   balas.                                                              *
 * ***********************************************************************/
public class SimulateFire : MonoBehaviour 
{

	public bool bSimulateBomb;          // Flag para saber si debe aparecer la parábola o no
	public bool bSimulateShot;          // Flag para saber si debe aparecer el disparo simulado
	public RaycastHit hitShot;          // Información del impacto de la bala.
	const float fLenghtSimShot = 3.0f;  // Longitud de cada disparo simulado
		
	const int iSegCount = 30;           // Número de segmentos que contendrá la parábola
	const float fSegLenght = 2.0f;      // Longitud de cada segmento
	Vector3[] vSegs;                    // Coordenadas de los segmentos que formarán la parábola
	Vector3 vSegVelocity;               // Velocidad en cada segmento
	float fSegTime;                     // Tiempo que tarda en recorrer la bomba cada segmento.

	Transform thisTransform;
    Transform gun;                      // Referencia al hijo Gun desde el cual se simulará el disparo.
	Transform grenades;                 // Referencia a Grenades desde donde se lanzarán las granadas.
	AttackUser attackUser;              // Componente Script Attack del objeto usuario

	LineRenderer sightLineShot;         // Linea que dibujará el efecto de disparar balas.
	LineRenderer sightLineBomb;         // Linea que dibujará la parábola de lanzamiento de bombas
	
	public LayerMask layerPathBomb;     // Capa a interactuar por path simulado de la bomba
	

	
	/*************************************************************************
	 * FUNCION: void SimulatePathBomb()                                      *
	 * DESCRIPCION: Muestra la parábola a seguir por la bomba que va a ser   *
	 *   lanzada.                                                            *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	void SimulatePathBomb()
	{		
		vSegs = new Vector3[iSegCount];
        // Posición de la cual parte la parábola de tiro	
		vSegs[0] = grenades.position;
		vSegVelocity = attackUser.vVelocityBomb * 1;    // Velocidad inicial de la parábola en el primer segmento.
		 
		for (int i = 1; i < iSegCount; i++)
		{						
			RaycastHit hit;
			if (Physics.Raycast( vSegs[i-1], vSegVelocity, out hit, fSegLenght, layerPathBomb))
			{
				fSegTime = hit.distance / vSegVelocity.magnitude;           // Tiempo que dura el desplazamiento del segmento 
				                                                            //  hasta la colisión				
				vSegVelocity += Physics.gravity * fSegTime;                 // Velocidad del siguiente segmento afectada por la gravedad				
				vSegs[i] = vSegs[i-1] + vSegVelocity * fSegTime;            // Posición del siguiente segmento que coincide con 
				                                                            //  el punto donde hay colisión				
				vSegVelocity = Vector3.Reflect( vSegVelocity, hit.normal);  // Se invierte la dirección debido al rebote por colisión
 			}
			else
			{
				fSegTime = fSegLenght / vSegVelocity.magnitude;   // Tiempo que durará el desplazamiento del segmento			
				vSegVelocity += (Physics.gravity * fSegTime);     // Velocidad del siguiente segmento afectada por la gravedad
				vSegs[i] = vSegs[i-1] + vSegVelocity * fSegTime;  // Posición del siguiente segmento.
			}
		}
		
		sightLineBomb.SetVertexCount( iSegCount);
		for (int i = 0; i < iSegCount; i++)
			sightLineBomb.SetPosition( i, vSegs[i]);
				
	}
	

		
	/*************************************************************************
	 * FUNCION: void SimulateShot()                                          *
	 * DESCRIPCION: Hace una simulación visual de los disparos de bala.      *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: Ninguno.                                                   *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void SimulateShot()
	{				
		// Dirección desde la pistola hasta el punto de impacto.
		Vector3 vGunPosition = gun.position + gun.renderer.bounds.extents.z * gun.forward;
		Vector3 vDirGun = hitShot.point - vGunPosition;
		vDirGun.Normalize();

		// Las balas simuladas no pueden ser de mayor longitud que la distancia al objeto impactado.
		float fTempLenghtSimShot = Mathf.Min( hitShot.distance, fLenghtSimShot);
		
		// Distancia aleatoria entre el objeto y la máxima distancia
		float fRandomDistance = Random.value * (hitShot.distance - fTempLenghtSimShot);
			
		sightLineShot.SetVertexCount( 2);
		sightLineShot.SetPosition( 0, vGunPosition + vDirGun * fRandomDistance);
		sightLineShot.SetPosition( 1, vGunPosition + vDirGun * (fRandomDistance + fTempLenghtSimShot));
								
	}
	
	
	
	// Use this for initialization
	void Start() 
	{		
		thisTransform = transform;
		gun = thisTransform.FindChild( "Gun");
		grenades = thisTransform.FindChild( "Grenades");
		attackUser = GetComponent<AttackUser>();
		
		// Se inicializa el LineRenderer.
		sightLineShot = gun.gameObject.AddComponent<LineRenderer>();			
		sightLineShot.material = new Material(Shader.Find("Particles/Additive"));
		sightLineBomb = grenades.gameObject.AddComponent<LineRenderer>();			
		sightLineBomb.material = new Material(Shader.Find("Particles/Additive"));		
		
		// Se inicializa el LineRenderer para los disparos
		Color startColor = Color.red;
		Color endColor = Color.yellow;
		startColor.a = 1;
		endColor.a = 0;
		sightLineShot.SetColors(startColor, endColor);
		sightLineShot.SetWidth(0.2f, 0.2f);

		// Se inicializa el LineRenderer para las bombas
		startColor = Color.yellow;
		endColor = Color.white;
		startColor.a = 1;
		endColor.a = 0;
		sightLineBomb.SetColors(startColor, endColor);
		sightLineBomb.SetWidth(0.1f, 0.1f);

		bSimulateBomb = false;  
		bSimulateShot = false;  
	}
	
	
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		if (bSimulateBomb)
			SimulatePathBomb();
		else 
			sightLineBomb.SetVertexCount( 0);			

		if (bSimulateShot)
			SimulateShot();	
		else
			sightLineShot.SetVertexCount( 0);				
	}
}
