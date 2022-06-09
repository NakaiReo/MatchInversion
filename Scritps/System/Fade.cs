using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public static Fade ins = null;

	[SerializeField] CanvasGroup canvasGroup;

	private void Awake()
	{
		if(ins == null)
		{
			ins = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// フェードの開始
	/// </summary>
	/// <param name="duration">フェードの時間</param>
	public void FadeIn(float duration)  => StartCoroutine(I_FadeIn(duration));

	/// <summary>
	/// フェードの終了
	/// </summary>
	/// <param name="duration">フェードの時間</param>
	public void FadeOut(float duration) => StartCoroutine(I_FadeOut(duration));

	//フェードインのアニメーション
	IEnumerator I_FadeIn(float duration)
	{
		canvasGroup.DOFade(0.0f, 0);
		canvasGroup.transform.DOScaleY(0, 0);
		canvasGroup.blocksRaycasts = true;
		canvasGroup.interactable = false;

		DOTween.Sequence()
			.Append(canvasGroup.DOFade(1.0f, duration))
			.Append(canvasGroup.transform.DOScaleY(1.0f, duration));

		yield break;
	}

	//フェードアウトのアニメーション
	IEnumerator I_FadeOut(float duration)
	{
		canvasGroup.DOFade(1.0f, 0);
		canvasGroup.transform.DOScaleY(1.0f, 0);
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
		DOTween.Sequence()
			.Append(canvasGroup.DOFade(0.0f, duration))
			.Append(canvasGroup.transform.DOScaleY(0.0f, duration));

		yield break;
	}
}
