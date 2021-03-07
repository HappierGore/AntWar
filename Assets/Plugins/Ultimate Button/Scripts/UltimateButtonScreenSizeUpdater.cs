/* Written by Kaz Crowe */
/* UltimateButtonScreenSizeUpdater.cs */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UltimateButtonScreenSizeUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		StartCoroutine( "YieldPositioning" );
	}

	IEnumerator YieldPositioning ()
	{
		yield return new WaitForEndOfFrame();

		UltimateButton[] allButtons = FindObjectsOfType( typeof( UltimateButton ) ) as UltimateButton[];
		for( int i = 0; i < allButtons.Length; i++ )
			allButtons[ i ].UpdatePositioning();
	}
}