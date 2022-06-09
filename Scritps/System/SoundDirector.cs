using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
///	AudioSoruce�������N���X
/// </summary>
public class Sounds
{
	public AudioSource audioSource;
	public string soundsDataPath;

	/// <summary>
	/// �N���X�̏�����
	/// </summary>
	public Sounds(AudioSource audioSource, string soundsDataPath)
	{
		this.audioSource = audioSource;
		this.soundsDataPath = soundsDataPath;
	}

	/// <summary>
	/// �����̍Đ�
	/// </summary>
	/// <param name="path">�����̃p�X</param>
	public void Play(string path)
	{
		AudioClip clip = Resources.Load(soundsDataPath + "/" + path) as AudioClip;
		audioSource.loop = false;
		audioSource.PlayOneShot(clip);
	}

	/// <summary>
	/// �����̃��[�v�Đ�
	/// </summary>
	/// <param name="path">�����̃p�X</param>
	public void PlayLoop(string path)
	{
		AudioClip clip = Resources.Load(soundsDataPath + "/" + path) as AudioClip;
		audioSource.loop = true;
		audioSource.clip = clip;
		audioSource.Play();
	}

	/// <summary>
	/// �����̒�~
	/// </summary>
	public void Stop()
	{
		audioSource.Stop();
	}
}

public class SoundDirector : MonoBehaviour
{
	public static SoundDirector ins = null;

	public Sounds BGM;
	public Sounds SE;
	[SerializeField] AudioMixer mixer; //�~�L�T�[

	private void Awake()
	{
		if (ins == null)
		{
			DontDestroyOnLoad(gameObject);
			ins = this;
		}
		else
		{
			Destroy(gameObject);
		}

		Init();
	}

	/// <summary>
	/// ������
	/// </summary>
	void Init()
	{
		AudioSource[] audioSource = GetComponents<AudioSource>();
		BGM = new Sounds(audioSource[0], "Sounds/BGM");
		SE = new Sounds(audioSource[1], "Sounds/SE");

		if (!PlayerPrefs.HasKey("Master")) PlayerPrefs.SetFloat("Master", 0.5f);
		if (!PlayerPrefs.HasKey("BGM")) PlayerPrefs.SetFloat("BGM", 0.5f);
		if (!PlayerPrefs.HasKey("SE")) PlayerPrefs.SetFloat("SE", 0.5f);

		Volume();
	}

	/// <summary>
	/// ���ʂ̕ۑ�
	/// </summary>
	public void Volume()
	{
		mixer.SetFloat("MasterVol", Pa2Db(PlayerPrefs.GetFloat("Master")));
		mixer.SetFloat("BGMVol", Pa2Db(PlayerPrefs.GetFloat("BGM")));
		mixer.SetFloat("SEVol", Pa2Db(PlayerPrefs.GetFloat("SE")));
	}

	/// <summary>
	/// �f�V�x���ϊ�
	/// </summary>
	/// <param name="pa"></param>
	/// <returns></returns>
	private float Pa2Db(float pa)
	{
		pa = Mathf.Clamp(pa, 0.0001f, 10f);
		return 20f * Mathf.Log10(pa);
	}

	/// <summary>
	/// �����ϊ�
	/// </summary>
	/// <param name="db"></param>
	/// <returns></returns>
	private float Db2Pa(float db)
	{
		db = Mathf.Clamp(db, -80f, 20f);
		return Mathf.Pow(10f, db / 20f);
	}
}
