using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorDirector
{
	public static Color RESULT_COLOR
	{ 
		get
		{
			Color resultColor;
			ColorUtility.TryParseHtmlString("#460F0F", out resultColor);
			return resultColor;		
		} 
	}
}