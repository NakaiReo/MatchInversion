using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CountDownTMP : MonoBehaviour
{
	public void Play(string text, float duration, float fadeTime, float paddingTime)
	{
		float mainTime = duration - fadeTime * 2 - paddingTime * 2 - 0.1f;

		var textMeshPro = GetComponent<TextMeshProUGUI>();
		var tmpAnimator = new DOTweenTMPAnimator(textMeshPro);

		textMeshPro.text = text;

		for(int i = 0; i < tmpAnimator.textInfo.characterCount; i++)
		{
			DOTween.Sequence()
				.Append(tmpAnimator.DORotateChar(i, Vector3.up * 90.0f, 0).SetEase(Ease.Linear))
				.Append(tmpAnimator.DORotateChar(i, Vector3.zero, fadeTime))
				.AppendInterval(mainTime)
				.Append(tmpAnimator.DORotateChar(i, Vector3.up * 90.0f, fadeTime).SetEase(Ease.Linear));
		}
	}

	public void Init()
	{
		GetComponent<TextMeshProUGUI>().text = "";
	}
}

