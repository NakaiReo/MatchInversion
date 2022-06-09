using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// タイムアウトのテキスト用のアニメーション
/// </summary>
public class TimeOutTMP : MonoBehaviour
{
	public void Play(float duration)
	{
		var textMeshPro = GetComponent<TextMeshProUGUI>();
		var tmpAnimator = new DOTweenTMPAnimator(textMeshPro);

		textMeshPro.text = "Time<br>Out!";

		for (int i = 0; i < tmpAnimator.textInfo.characterCount; i++)
		{
			tmpAnimator.DOScaleChar(i, 0.0f, 0);
			tmpAnimator.DOFadeChar(i, 0.0f, 0);
			Vector3 currCharOffset = tmpAnimator.GetCharOffset(i);
			DOTween.Sequence()
				.Append(tmpAnimator.DOOffsetChar(i, currCharOffset + new Vector3(0, 30, 0), duration).SetEase(Ease.OutFlash, 2))
				.Join(tmpAnimator.DOFadeChar(i, 1, duration))
				.Join(tmpAnimator.DOScaleChar(i, 1, duration).SetEase(Ease.OutBack))
				.SetDelay(0.07f * i);
		}
	}

	public void Fade(float duration)
	{
		var textMeshPro = GetComponent<TextMeshProUGUI>();
		var tmpAnimator = new DOTweenTMPAnimator(textMeshPro);

		textMeshPro.text = "Time<br>Out!";

		for (int i = 0; i < tmpAnimator.textInfo.characterCount; i++)
		{
			tmpAnimator.DOScaleChar(i, 1f, 0);
			Vector3 currCharOffset = tmpAnimator.GetCharOffset(i);
			DOTween.Sequence()
				.Append(tmpAnimator.DOOffsetChar(i, currCharOffset + new Vector3(0, 30, 0), duration).SetEase(Ease.OutFlash, 2))
				.Join(tmpAnimator.DOFadeChar(i, 0, duration))
				.Join(tmpAnimator.DOScaleChar(i, 0, duration).SetEase(Ease.OutBack))
				.SetDelay(0.07f * i);
		}
	}

	public void Init()
	{
		GetComponent<TextMeshProUGUI>().text = "";
	}
}

