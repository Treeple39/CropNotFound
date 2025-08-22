using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MTextShaderCharAnimation : MonoBehaviour
{
    public enum TextAnimationType
    {
        Wave,
    }

    public TextAnimationType animationType = TextAnimationType.Wave;

    [Header("Wave Param")]
    public float waveStrength = 1;
    public float waveFreq = 1;
    public float waveSpeed = 1;

    private TMP_Text mText;
    private string lastText;
    private void Awake()
    {
        mText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if(mText != null)
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(UpdateInfo);
        }
    }

    private void OnDisable()
    {
        if (mText != null)
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(UpdateInfo);
        }
    }

    private void Start()
    {
        UpdateInfo(mText);
    }

    private void UpdateInfo(UnityEngine.Object obj)
    {
        if(obj != mText || lastText == mText.text)
        {
            return;
        }

        lastText = mText.text;
        switch (animationType)
        {
            case TextAnimationType.Wave:
                UpdateWaveInfo();
                break;
        }
    }

    private void UpdateWaveInfo()
    {
        mText.ForceMeshUpdate();

        int charCount = mText.textInfo.characterCount;
        if (charCount == 0)
            return;

        /// <summary>
        /// Recover UV2
        /// </summary>
        
        for (int i = 0; i < charCount; i++)
        {
            TMP_CharacterInfo charInfo = mText.textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            float xValue = i / (float)charCount;

            var matIndex = charInfo.materialReferenceIndex;
            var uv2s = mText.textInfo.meshInfo[matIndex].uvs2;

            //Control 4 angle point
            uv2s[vertexIndex + 0].x = xValue;
            uv2s[vertexIndex + 1].x = xValue;
            uv2s[vertexIndex + 2].x = xValue;
            uv2s[vertexIndex + 3].x = xValue;
        }

        mText.UpdateVertexData(TMP_VertexDataUpdateFlags.Uv2);
        mText.fontMaterial.SetFloat("_CharNum", charCount);
        mText.fontMaterial.SetFloat("_WaveStrength", waveStrength);
        mText.fontMaterial.SetFloat("_WaveFreq", waveFreq);
        mText.fontMaterial.SetFloat("_WaveSpeed", waveSpeed);
    }
}
