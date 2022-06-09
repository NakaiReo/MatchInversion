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
	/// �t�F�[�h�̊J�n
	/// </summary>
	/// <param name="duration">�t�F�[�h�̎���</param>
	public void FadeIn(float duration)  => StartCoroutine(I_FadeIn(duration));

	/// <summary>
	/// �t�F�[�h�̏I��
	/// </summary>
	/// <param name="duration">�t�F�[�h�̎���</param>
	public void FadeOut(float duration) => StartCoroutine(I_FadeOut(duration));

	//�t�F�[�h�C���̃A�j���[�V����
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

	//�t�F�[�h�A�E�g�̃A�j���[�V����
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
