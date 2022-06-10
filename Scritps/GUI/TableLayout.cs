using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �Ֆʂ̑傫������������
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class TableLayout : MonoBehaviour
{
	[SerializeField] GridLayoutGroup gridLayout;

	public int gridSize; //�O���b�h�̑傫��
	public float space;  //�O���b�h���Ƃ̌���

	private void OnValidate()
	{
		if(gridLayout == null) gridLayout = GetComponent<GridLayoutGroup>();
		Layout();
	}

	/// <summary>
    /// �Ֆʂ̑傫����ݒ�
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
