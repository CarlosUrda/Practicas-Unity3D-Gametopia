using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour 
{
	public int iMaxEnergy = 100;  // Energía máxima posible del objeto
	int iEnergy;                  // Energía actual del objeto
	
	
	
	/*************************************************************************
	 * FUNCION: public float GetEnergy()                                     *
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

	
	
	/*************************************************************************
	 * FUNCION: public void ChangeEnergy( int iShiftEnergy)                  *
	 * DESCRIPCION: Cambia el valor de la energía actual del objeto al       *
	 *   añadirle el valor pasado por argumento.                             *
	 * ARGUMENTOS:                                                           *
	 *   ENTRADA: int iEnergy: Valor a añadir a la energía del objeto.       *
	 *   SALIDA: Ninguno.                                                    *
	 * RETORNO: Ninguno                                                      *
	 *************************************************************************/
	public void ChangeEnergy( int iShiftEnergy)
	{
		iEnergy += iShiftEnergy;
	}

	
	
	// Use this for initialization
	void Start () 
	{
		iEnergy = iMaxEnergy;   // El objeto empieza con energía máxima
	
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
