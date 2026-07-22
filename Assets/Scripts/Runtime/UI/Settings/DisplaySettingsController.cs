using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DisplaySettingsController : MonoBehaviour
{
    [Header("Brightness")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Text brightnessValueText;
    [SerializeField] private RectTransform brightnessHandle;

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeValueText;
    [SerializeField] private RectTransform masterVolumeHandle;

    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private TMP_Text bgmVolumeValueText;
    [SerializeField] private RectTransform bgmVolumeHandle;

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeValueText;
    [SerializeField] private RectTransform sfxVolumeHandle;

    [Header("Brightness Processing")]
    [SerializeField] private Volume brightnessVolume;
    [SerializeField] private float minBrightness = -1.5f;
    [SerializeField] private float maxBrightness = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParameter = "MasterVolume";
    [SerializeField] private string bgmVolumeParameter = "BGMVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    private ColorAdjustments colorAdjustments;

    private const string BrightnessKey = "Settings.Brightness";
    private const string MasterVolumeKey = "Settings.MasterVolume";
    private const string BGMVolumeKey = "Settings.BGMVolume";
    private const string SFXVolumeKey = "Settings.SFXVolume";

    private const float DefaultBrightness = 50f;
    private const float DefaultMasterVolume = 100f;
    private const float DefaultBGMVolume = 100f;
    private const float DefaultSFXVolume = 100f;

    private void Start()
    {
        if (brightnessVolume != null)
        {
            brightnessVolume.profile.TryGet(out colorAdjustments);
        }

        brightnessSlider.onValueChanged.AddListener(UpdateBrightnessText);
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolumeText);
        bgmVolumeSlider.onValueChanged.AddListener(UpdateBGMVolumeText);
        sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolumeText);

        LoadSettings();
    }
    
    private void UpdateBrightnessText(float value)
    {
        UpdateValueText(brightnessHandle, brightnessValueText, value);
        ApplyBrightness(value);
    }

    private void UpdateMasterVolumeText(float value)
    {
        UpdateValueText(masterVolumeHandle, masterVolumeValueText, value);
        ApplyMasterVolume(value);
    }

    private void UpdateBGMVolumeText(float value)
    {
        UpdateValueText(bgmVolumeHandle, bgmVolumeValueText, value);
        ApplyBGMVolume(value);
    }

    private void UpdateSFXVolumeText(float value)
    {
        UpdateValueText(sfxVolumeHandle, sfxVolumeValueText, value);
        ApplySFXVolume(value);
    }

    private void UpdateValueText(RectTransform handle, TMP_Text valueText, float value)
    {
        valueText.text = Mathf.RoundToInt(value).ToString();

        RectTransform valueTextRect = valueText.rectTransform;
        RectTransform parentRect = valueTextRect.parent as RectTransform;

        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, handle.position, null, out localPos);

        Vector2 currentPos = valueTextRect.anchoredPosition;
        valueTextRect.anchoredPosition = new Vector2(localPos.x, currentPos.y);
    }

    private void RefreshAllTexts()
    {
        UpdateBrightnessText(brightnessSlider.value);
        UpdateMasterVolumeText(masterVolumeSlider.value);
        UpdateBGMVolumeText(bgmVolumeSlider.value);
        UpdateSFXVolumeText(sfxVolumeSlider.value);
    }

    private void LoadSettings()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat(BrightnessKey, DefaultBrightness);
        masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat(BGMVolumeKey, DefaultBGMVolume);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, DefaultSFXVolume);

        RefreshAllTexts();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(BrightnessKey, brightnessSlider.value);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(BGMVolumeKey, bgmVolumeSlider.value);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolumeSlider.value);
        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        brightnessSlider.value = DefaultBrightness;
        masterVolumeSlider.value = DefaultMasterVolume;
        bgmVolumeSlider.value = DefaultBGMVolume;
        sfxVolumeSlider.value = DefaultSFXVolume;

        RefreshAllTexts();
    }

    public void DiscardChanges()
    {
        LoadSettings();
    }

    private void ApplyBrightness(float value)
    {
        if (colorAdjustments == null)
        {
            return;
        }

        float normalizedValue = value / 100f;
        float brightnessValue = Mathf.Lerp(minBrightness, maxBrightness, normalizedValue);
        colorAdjustments.postExposure.value = brightnessValue;
    }

    private void ApplyMasterVolume(float value)
    {
        if (audioMixer == null)
        {
            return;
        }

        audioMixer.SetFloat(masterVolumeParameter, ConvertVolumeToDB(value));
    }

    private void ApplyBGMVolume(float value)
    {
        if (audioMixer == null)
        {
            return;
        }

        audioMixer.SetFloat(bgmVolumeParameter, ConvertVolumeToDB(value));
    }

    private void ApplySFXVolume(float value)
    {
        if (audioMixer == null)
        {
            return;
        }

        audioMixer.SetFloat(sfxVolumeParameter, ConvertVolumeToDB(value));
    }

    private float ConvertVolumeToDB(float value)
    {
        if (value <= 0f)
        {
            return -80f;
        }

        return Mathf.Log10(value / 100f) * 20f;
    }

}
