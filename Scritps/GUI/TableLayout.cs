using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 盤面の大きさを処理する
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class TableLayout : MonoBehaviour
{
	[SerializeField] GridLayoutGroup gridLayout;

	public int gridSize; //グリッドの大きさ
	public float space;  //グリッドごとの隙間

	private void OnValidate()
	{
		if(gridLayout == null) gridLayout = GetComponent<GridLayoutGroup>();
		Layout();
	}

	/// <summary>
    /// 盤面の大きさを設定
    /// </summary>
	public void Layout()
	{
		RectTransform rectTransform = GetComponent<RectTransform>();
		float rectSize = rectTransform.rect.width;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectSize);

		float cellSize = rectSize / gridSize - space * (gridSize - 1);

		gridLayout.cellSize = Vector2.one * cellSize;
		gridLayout.spacing  = Vector2.one * space;
	}
}
