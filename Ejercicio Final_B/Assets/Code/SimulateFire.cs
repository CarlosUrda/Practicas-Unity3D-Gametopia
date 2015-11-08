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

	LineRenderer sightLine;             // Linea que dibujará la parábola de tiro.
	public bool bSimulateBomb;          // Flag para saber si debe aparecer la parábola o no
	public bool bSimulateShot;          // Flag para saber si debe aparecer el disparo simulado
	public Vector3 vForceBomb;          // Fuerza inicial con que salen las bombas disparadas.
	public float fDistanceSimShot;      // Distancia que alcanza la simulación de disparo. 
	                                    //  Se usa para que la simulación visual no traspase objetos.	
	Transform thisTransform;


	
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
		const int iSegCount = 35;           // Número de segmentos que contendrá la parábola
		const float fSegLenght = 1.0f;      // Longitud de cada segmento
		Vector3[] vSegs;                    // Coordenadas de los segmentos que formarán la parábola
		Vector3 vSegVelocity;               // Velocidad en cada segmento
		float fSegTime;                     // Tiempo que tarda en recorrer la bomba cada segmento.
		
		
		vSegs = new Vector3[iSegCount];
		vSegs[0] = thisTransform.position;            // Posición de la cual parte la parábola de tiro	
		vSegVelocity = vForceBomb * Time.deltaTime;   // Velocidad inicial de la parábola en el primer segmento.
		 
		for (int i = 1; i < iSegCount; i++)
		{			
			RaycastHit hit;
			if (Physics.Raycast( vSegs[i-1], vSegVelocity, out hit, fSegLenght))
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
		
		// Set the colour of our path to the colour of the next ball
		sightLine.SetVertexCount( iSegCount);
		for (int i = 0; i < iSegCount; i++)
			sightLine.SetPosition( i, vSegs[i]);
				
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
		const float fLenghtSimShot = 20.0f;   // Longitud de cada disparo simulado
		
		// Distancia aleatoria entre el objeto y la máxima distancia
		float fRandomDistance = Random.value * (DistanceSimShot - fLenghtSimShot);
			
		// Set the colour of our path to the colour of the next ball
		sightLine.SetWidth(0.2f, 0.2f);
		sightLine.SetVertexCount( 2);
		sightLine.SetPosition( 0, thisTransform.position + thisTransform.forward * fRandomDistance);
		sightLine.SetPosition( 1, thisTransform.position + thisTransform.forward * (fRandomDistance + fLenghtSimShot));
								
	}
	
	
	
	// Use this for initialization
	void Start() 
	{
		// Se inicializa el LineRenderer.
		sightLine = gameObject.AddComponent<LineRenderer>();			
		sightLine.material = new Material(Shader.Find("Particles/Additive"));
		Color startColor = Color.red;
		Color endColor = Color.yellow;
		startColor.a = 1;
		endColor.a = 0;
		sightLine.SetColors(startColor, endColor);
		sightLine.SetWidth(0.1f, 0.1f);
		
		thisTransform = transform;
		
		bSimulateBomb = false;  
		bSimulateShot = false;  
	}
	
	
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		if (bSimulateBomb)
		{
			SimulatePathBomb();
		}
		else if (bSimulateShot)
		{
			SimulateFire();	
		}
		else
			sightLine.SetVertexCount( 0);			

	
	}
}
