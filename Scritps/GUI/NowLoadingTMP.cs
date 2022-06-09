using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// NowLoadingテキスト用のアニメーション
/// </summary>
public class NowLoadingTMP : MonoBehaviour
{
	[SerializeField] float delayTime;
	[SerializeField] float fadeDuration;
	[SerializeField] float rotateDuration;
	[SerializeField] float intervalDuration;

	private void Start()
	{
		var textMeshPro = GetComponent<TextMeshProUGUI>();
		var tmpAnimator = new DOTweenTMPAnimator(textMeshPro);

		int length = tmpAnimator.textInfo.characterCount;

		for (int i = 0; i < tmpAnimator.textInfo.characterCount; i++)
		{
			tmpAnimator.DORotateChar(i, Vector3.up * 90, 0);

			Sequence sequence = DOTween.Sequence()
				.Append(tmpAnimator.DORotateChar(i, Vector3.up * 0, fadeDuration).SetEase(Ease.OutSine))
				.Join(tmpAnimator.DOColorChar(i, Color.white, fadeDuration))
				.AppendInterval(fadeDuration + intervalDuration)
				.Append(tmpAnimator.DORotateChar(i, Vector3.up * 90, rotateDuration).SetEase(Ease.InSine))
				.AppendInterval(rotateDuration)

				.Append(tmpAnimator.DORotateChar(i, Vector3.up * 0, fadeDuration).SetEase(Ease.OutSine))
				.Join(tmpAnimator.DOColorChar(i, Color.white, fadeDuration))
				.AppendInterval(fadeDuration + intervalDuration)
				.Append(tmpAnimator.DORotateChar(i, Vector3.up * 90, rotateDuration).SetEase(Ease.InSine))
				.AppendInterval(rotateDuration)
				.SetDelay(delayTime * i, false)
				.SetLoops(-1, LoopType.Restart);

			//sequence.SetDelay(delayTime * i).Play();
		}
	}
}
