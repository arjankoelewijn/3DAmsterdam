using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedOrder : MonoBehaviour
{
	[SerializeField]
	private Transform[] orderOfItems;
	
    void OnTransformChildrenChanged(){
		for (int i = 0; i < orderOfItems.Length; i++)
		{
			orderOfItems[i].SetSiblingIndex(i);
		}
	}
}
