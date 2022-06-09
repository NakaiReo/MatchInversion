using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingMenu : MonoBehaviour
{
    public static SettingMenu ins = null;

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

    [SerializeField] CanvasGroup canvasGroup; 
    [SerializeField] RectTransform panel;
    [SerializeField] GameObject buttonsGroup;

    [Space]
    [SerializeField] Slider masterSlider; //Master�̑傫��
    [SerializeField] Slider bgmSlider;    //BGM�̑傫��
    [SerializeField] Slider seSlider;     //SE�̑傫��

    public bool useButtons;  //�{�^����\�������邩
    bool isMenuOpen = false; //���j���[���J���ꂢ�邩

    float animTime = 0.25f; //�A�j���[�V�����̎���

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Master");
        bgmSlider.value    = PlayerPrefs.GetFloat("BGM");
        seSlider.value     = PlayerPrefs.GetFloat("SE");
    }

    public void ToggleActive()
    {
        isMenuOpen = !isMenuOpen;
        SetActive(enabled);
    }

    public void SetActive(bool enable)
    {
        if (enable) OnMenuEnable();
        else OnMenuDisable();
    }

    /// <summary>
    /// �{�^���̏���: ���j���[���J��
    /// </summary>
    public void OnMenuEnable()
    {
        enabled = true;

        buttonsGroup.SetActive(useButtons);

        canvasGroup.DOFade(1.0f, animTime);
        panel.DOScaleX(1.0f, animTime);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    /// <summary>
    /// �{�^���̏���: ���j���[�����
    /// </summary>
    public void OnMenuDisable()
    {
        enabled = false;

        SoundDirector.ins.SE.Play("Select");

        PlayerPrefs.SetFloat("Master", masterSlider.value);
        PlayerPrefs.SetFloat("BGM", bgmSlider.value);
        PlayerPrefs.SetFloat("SE", seSlider.value);
        SoundDirector.ins.Volume();
        PlayerPrefs.Save();

        canvasGroup.DOFade(0.0f, animTime);
        panel.DOScaleX(0.0f, animTime);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    /// <summary>
    /// �{�^���̏���: ���j���[�����
    /// </summary>
    public void OnCloseMenu()
    {
        OnMenuDisable();
    }


    ////--------------------------------------------------------------
    //// ���ꂼ���Volume��ۑ�����
    ////--------------------------------------------------------------
    //public void SetMasterVolume(Slider slider)
    //{
    //    PlayerPrefs.SetFloat("Master", slider.value);
    //    PlayerPrefs.Save();
    //    SoundDirector.ins.Volume();
    //}

    //public void SetBGMVolume(Slider slider)
    //{
    //    PlayerPrefs.SetFloat("BGM", slider.value);
    //    PlayerPrefs.Save();
    //    SoundDirector.ins.Volume();
    //}

    //public void SetSEVolume(Slider slider)
    //{
    //    PlayerPrefs.SetFloat("SE", slider.value);
    //    PlayerPrefs.Save();
    //    SoundDirector.ins.Volume();
    //}
}
