using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class TableLayout : MonoBehaviour
{
	[SerializeField] GridLayoutGroup gridLayout;

	public int gridSize;
	public float space;

	private void OnValidate()
	{
		if(gridLayout == null) gridLayout = GetComponent<GridLayoutGroup>();
		Layout();
	}

	public void Layout()
	{
		RectTransform rectTransform = GetComponent<RectTransform>();
		float rectSize = rectTransform.rect.width;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectSize);

		Debug.Log(rectSize);
		float cellSize = rectSize / gridSize - space * (gridSize - 1);
		Debug.Log(cellSize);

		gridLayout.cellSize = Vector2.one * cellSize;
		gridLayout.spacing  = Vector2.one * space;
	}
}
